using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{
    [SerializeField]
    PlayerManager playerManager;
    [SerializeField]
    GameObject camMain, camBattle;

    public FieldData fieldData;

    public float timePause = 1;

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE,
        NOTINBATTLE
    }
    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<CharacterStateMachine> PartyMembers = new List<CharacterStateMachine>();
    public List<GameObject> CharactersInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    List<GameObject> enemyButtons = new List<GameObject>();
    List<GameObject> allyButtons = new List<GameObject>();

    ObjectPool objectPool;

    public enum CharacterUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public CharacterUI CharacterInput;

    public List<GameObject> CharactersToManage = new List<GameObject>();
    HandleTurn CharacterChoice;

    string targetButtonTag = "TargetButton";
    public Transform targetSpacer;

    public GameObject attackPanel, enemySelectPanel, skillPanel;

    string actionButtonTag = "ActionButton", skillButtonTag = "SkillButton";
    public Transform actionSpacer, skillSpacer;
    [SerializeField]
    List<GameObject> attackButtons = new List<GameObject>();

    public List<Transform> allySpawnPoints = new List<Transform>();
    public List<Transform> enemySpawnPoints = new List<Transform>();

    private void Start()
    {
        objectPool = ObjectPool.instance;
    }

    public void BattleStart()
    {
        camBattle.SetActive(true);
        camMain.SetActive(false);

        int enemyAmount = Random.Range(1, fieldData.maxEnemies + 1);
        for (int i = 0; i < enemyAmount; i++)
        {
            PossibleEnemies possibleEnemies = fieldData.possibleEnemies[Random.Range(0, fieldData.possibleEnemies.Count)];
            enemySpawnPoints[i].GetChild(0).GetComponent<EnemyStateMachine>().DataLoad(possibleEnemies.possibleEnemies[Random.Range(0, possibleEnemies.possibleEnemies.Count)].GetComponent<EnemyStateMachine>().enemy);
            EnemiesInBattle.Add(enemySpawnPoints[i].gameObject);
            enemySpawnPoints[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < PartyMembers.Count; i++)
        {
            allySpawnPoints[i].GetChild(0).GetComponent<CharacterStateMachine>().DataLoad(PartyMembers[i].character);
            CharactersInBattle.Add(allySpawnPoints[i].gameObject);
            allySpawnPoints[i].gameObject.SetActive(true);
        }

        battleStates = PerformAction.WAIT;
        CharacterInput = CharacterUI.ACTIVATE;

        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        skillPanel.SetActive(false);

        EnemyButtons();
        AllyButtons();

        timePause = 1;
    }

    private void Update()
    {
        switch(battleStates)
        {
            case (PerformAction.WAIT):
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = PerformList[0].AttacksGameObject;
                if (PerformList[0].Type.Equals("Enemy"))
                {
                    EnemyStateMachine enemyStateMachine = performer.GetComponent<EnemyStateMachine>();
                    switch(PerformList[0].chosenAttack.target)
                    {
                        case Target.ENEMY:
                            for (int i = 0; i < CharactersInBattle.Count; i++)
                            {
                                if (PerformList[0].AttackersTarget.Equals(CharactersInBattle[i]))
                                {
                                    enemyStateMachine.attackTarget = PerformList[0].AttackersTarget;
                                    enemyStateMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                                    break;
                                }
                                else
                                {
                                    PerformList[0].AttackersTarget = CharactersInBattle[Random.Range(0, CharactersInBattle.Count)];
                                    enemyStateMachine.attackTarget = PerformList[0].AttackersTarget;
                                    enemyStateMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                                }
                            }
                            break;
                        case Target.ALLY:
                            for (int i = 0; i < EnemiesInBattle.Count; i++)
                            {
                                if (PerformList[0].AttackersTarget.Equals(EnemiesInBattle[i]))
                                {
                                    enemyStateMachine.attackTarget = PerformList[0].AttackersTarget;
                                    enemyStateMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                                    break;
                                }
                                else
                                {
                                    PerformList[0].AttackersTarget = EnemiesInBattle[Random.Range(0, EnemiesInBattle.Count)];
                                    enemyStateMachine.attackTarget = PerformList[0].AttackersTarget;
                                    enemyStateMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                                }
                            }
                            break;
                    }
                }
                else if (PerformList[0].Type.Equals("Character"))
                {
                    CharacterStateMachine characterStateMachine = performer.GetComponent<CharacterStateMachine>();
                    characterStateMachine.attackTarget = PerformList[0].AttackersTarget;
                    characterStateMachine.currentState = CharacterStateMachine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                break;
            case (PerformAction.CHECKALIVE):
                if (CharactersInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                }
                else if(EnemiesInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                }
                else
                {
                    ClearAttackPanel();
                    CharacterInput = CharacterUI.ACTIVATE;
                }
                break;
            case (PerformAction.LOSE):

                break;
            case (PerformAction.WIN):

                break;
            case (PerformAction.NOTINBATTLE):

                break;
        }

        switch (CharacterInput)
        {
            case (CharacterUI.ACTIVATE):
                if (CharactersToManage.Count > 0)
                {
                    CharactersToManage[0].transform.GetChild(0).gameObject.SetActive(true);
                    CharacterChoice = new HandleTurn();

                    attackPanel.SetActive(true);
                    CreateAttackButtons();
                    CharacterInput = CharacterUI.WAITING;
                }
                break;
            case (CharacterUI.WAITING):
                break;
            case (CharacterUI.DONE):
                CharacterInputDone();
                break;
        }
    }
    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }

    public void EnemyButtons()
    { 
        foreach(GameObject button in enemyButtons)
        {
            button.SetActive(false);
        }
        enemyButtons.Clear();

        foreach(GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = objectPool.Spawn(targetButtonTag);
            newButton.tag = "Enemy";
            TargetSelectButton button = newButton.GetComponent<TargetSelectButton>();

            EnemyStateMachine currentEnemy = enemy.transform.GetChild(0).GetComponent<EnemyStateMachine>();

            TextMeshProUGUI buttonText = newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonText.text = currentEnemy.enemy.name;

            button.targetPrefab = enemy;

            newButton.transform.SetParent(targetSpacer, false);
            enemyButtons.Add(newButton.gameObject);
        }
    }

    public void AllyButtons()
    {
        foreach (GameObject button in allyButtons)
        {
            button.SetActive(false);
        }
        allyButtons.Clear();

        foreach (GameObject ally in CharactersInBattle)
        {
            GameObject newButton = objectPool.Spawn(targetButtonTag);
            newButton.tag = "Character";
            TargetSelectButton button = newButton.GetComponent<TargetSelectButton>();

            CharacterStateMachine currentAlly = ally.transform.GetChild(0).GetComponent<CharacterStateMachine>();

            TextMeshProUGUI buttonText = newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonText.text = currentAlly.character.name;

            button.targetPrefab = ally;

            newButton.transform.SetParent(targetSpacer, false);
            allyButtons.Add(newButton.gameObject);
        }
    }

    public void Input2(GameObject chosenTarget)
    {
        CharacterChoice.AttackersTarget = chosenTarget;
        CharacterInput = CharacterUI.DONE;
    }

    void CharacterInputDone()
    {
        PerformList.Add(CharacterChoice);

        ClearAttackPanel();

        CharactersToManage[0].transform.GetChild(0).gameObject.SetActive(false);
        CharactersToManage.RemoveAt(0);
        CharacterInput = CharacterUI.ACTIVATE;
    }

    void ClearAttackPanel()
    {
        enemySelectPanel.SetActive(false);
        attackPanel.SetActive(false);
        skillPanel.SetActive(false);

        foreach(GameObject button in attackButtons)
        {
            button.SetActive(false);
        }
        attackButtons.Clear();
    }

    void CreateAttackButtons()
    {
        GameObject skillBtn = objectPool.Spawn(actionButtonTag);
        TextMeshProUGUI skillBtnText = skillBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        skillBtnText.text = "Attack";
        skillBtn.GetComponent<Button>().onClick.AddListener(() => Input3());
        skillBtn.transform.SetParent(actionSpacer, false);
        attackButtons.Add(skillBtn);

        if (CharactersToManage[0].GetComponent<CharacterStateMachine>().character.skills.Count > 0)
        {
            foreach(BaseAttack skillAttack in CharactersToManage[0].GetComponent<CharacterStateMachine>().character.skills)
            {
                GameObject newSkillButton = objectPool.Spawn(skillButtonTag);
                TextMeshProUGUI newSkillButtonText = newSkillButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                newSkillButtonText.text = skillAttack.attackName;
                AttackButton newAttackButton = newSkillButton.GetComponent<AttackButton>();
                newAttackButton.attackToPerform = skillAttack;
                newSkillButton.transform.SetParent(skillSpacer, false);
                attackButtons.Add(newSkillButton);
            }
        }
        else
        {
            skillBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void Input3()
    {
        attackPanel.SetActive(false);
        skillPanel.SetActive(true);
        for (int i = 0; i < skillSpacer.childCount; i++)
        {
            if (skillSpacer.GetChild(i).GetComponent<AttackButton>().attackToPerform.attackCost > CharactersToManage[0].GetComponent<CharacterStateMachine>().character.currentMp)
                skillSpacer.GetChild(i).GetComponent<Button>().interactable = false;
            else
                skillSpacer.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    public void Input4(BaseAttack chosenSkill)
    {
        CharacterChoice.Attacker = CharactersToManage[0].name;
        CharacterChoice.AttacksGameObject = CharactersToManage[0];
        CharacterChoice.Type = "Character";

        CharacterChoice.chosenAttack = chosenSkill;
        skillPanel.SetActive(false);
        for (int i = 0; i < targetSpacer.childCount; i++)
        {
            switch (chosenSkill.target)
            {
                case Target.ENEMY:
                    if (targetSpacer.GetChild(i).CompareTag("Character"))
                        targetSpacer.GetChild(i).gameObject.SetActive(false);
                    if (targetSpacer.GetChild(i).CompareTag("Enemy"))
                        targetSpacer.GetChild(i).gameObject.SetActive(true);
                    break;
                case Target.ALLY:
                    if (targetSpacer.GetChild(i).CompareTag("Character"))
                        targetSpacer.GetChild(i).gameObject.SetActive(true);
                    if (targetSpacer.GetChild(i).CompareTag("Enemy"))
                        targetSpacer.GetChild(i).gameObject.SetActive(false);
                    break;
            }
        }
        enemySelectPanel.SetActive(true);
    }

    public void BattleResult()
    {
        timePause = 0;
        for (int i = 0; i < CharactersInBattle.Count; i++)
        {
            CharactersInBattle[i].transform.GetChild(0).GetComponent<CharacterStateMachine>().currentState = CharacterStateMachine.TurnState.WAITING;
        }
        BattleEnd();
        battleStates = PerformAction.NOTINBATTLE;
    }

    void BattleEnd()
    {
        camBattle.SetActive(false);
        camMain.SetActive(true);

        playerManager.timePause = 1;

        for (int i = 0; i < allySpawnPoints.Count; i++)
            allySpawnPoints[i].gameObject.SetActive(false);
        for (int i = 0; i < enemySpawnPoints.Count; i++)
            enemySpawnPoints[i].gameObject.SetActive(false);

        CharactersInBattle.Clear();
        EnemiesInBattle.Clear();
        PerformList.Clear();
        ClearAttackPanel();
    }
}
