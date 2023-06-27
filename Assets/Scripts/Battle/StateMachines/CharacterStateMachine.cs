using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterStateMachine : MonoBehaviour
{
    public BattleStateMachine battleStateMachine;
    public BaseCharacter character;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD,
        DATA
    }

    public TurnState currentState;

    private float currentCooldown = 0f, maxCooldown = 5;
    public Image progressBar;
    public GameObject selector;

    public GameObject attackTarget;
    private bool actionStarted = false;

    bool alive = true;
    AllyPanelStats stats;
    public GameObject allyPanel;
    public Transform panelSpacer;

    [SerializeField]
    PerfectPixelWithZoom cam;

    [SerializeField]
    GameObject attackCanvas;
    ObjectPool objectPool;
    [SerializeField]
    Transform upperNotes, belowNotes;
    [SerializeField]
    LineManager lineManager;
    [SerializeField]
    RectTransform background;

    private void Start()
    {
        objectPool = ObjectPool.instance;
    }

    private void OnEnable()
    {
        alive = true;
        if (character.currentHp <= 0)
            character.currentHp = 1;

        gameObject.tag = "Character";
        if (!currentState.Equals(TurnState.DATA))
        {
            currentState = TurnState.PROCESSING;

            CreateAllyPanel();
            GetComponent<Image>().sprite = character.sprite;
            GetComponent<Image>().SetNativeSize();

            selector.SetActive(false);
        }
    }

    public void DataLoad(BaseCharacter characterData)
    {
        currentCooldown = 0;

        character = characterData;

        for (int i = 0; i < character.skillsLearn.Count; i++)
        {
            if (character.level >= character.skillsLearn[i].level)
                character.skills.Add(character.skillsLearn[i].attack);
        }
    }

    private void Update()
    {
        switch(currentState)
        {
            case (TurnState.PROCESSING):
                UpdateProgressBar();
                break;
            case (TurnState.ADDTOLIST):
                battleStateMachine.CharactersToManage.Add(gameObject);
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                break;
            case (TurnState.SELECTING):
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    gameObject.tag = "DeadCharacter";
                    battleStateMachine.CharactersInBattle.Remove(transform.parent.gameObject);
                    battleStateMachine.CharactersToManage.Remove(transform.parent.gameObject);
                    selector.SetActive(false);
                    battleStateMachine.attackPanel.SetActive(false);
                    battleStateMachine.enemySelectPanel.SetActive(false);

                    if (battleStateMachine.CharactersInBattle.Count > 0)
                    {
                        for (int i = 0; i < battleStateMachine.PerformList.Count; i++)
                        {
                            if (battleStateMachine.PerformList[i].AttacksGameObject.Equals(transform.parent.gameObject))
                            {
                                battleStateMachine.PerformList.Remove(battleStateMachine.PerformList[i]);
                            }
                            if (battleStateMachine.PerformList[i].AttackersTarget.Equals(gameObject))
                            {
                                battleStateMachine.PerformList[i].AttackersTarget = battleStateMachine.CharactersInBattle[Random.Range(0, battleStateMachine.CharactersInBattle.Count)];
                            }
                        }
                    }
                    //색 바꾸기

                    battleStateMachine.AllyButtons();
                    battleStateMachine.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
                break;
        }
    }

    void UpdateProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime * character.speed * .2f * battleStateMachine.timePause;
        float calculate = currentCooldown / maxCooldown;

        progressBar.transform.localScale = new Vector2(Mathf.Clamp(calculate, 0, 1), 1);

        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
            battleStateMachine.timePause = 0;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        StartCoroutine(DoDamage());
        yield return new WaitForSeconds(5f);

        battleStateMachine.PerformList.RemoveAt(0);
        if (!battleStateMachine.battleStates.Equals(BattleStateMachine.PerformAction.WIN) && !battleStateMachine.battleStates.Equals(BattleStateMachine.PerformAction.LOSE))
        {
            battleStateMachine.battleStates = BattleStateMachine.PerformAction.WAIT;

            currentCooldown = 0f;
            currentState = TurnState.PROCESSING;
            battleStateMachine.timePause = 1;
        }
        else
        {
            battleStateMachine.battleStates = BattleStateMachine.PerformAction.WAIT;

            currentCooldown = 0f;
            currentState = TurnState.PROCESSING;
            battleStateMachine.timePause = 0;
            yield return new WaitForSeconds(2f);
            currentState = TurnState.WAITING;
            battleStateMachine.BattleResult();
        }
        actionStarted = false;
    }

    public void TakeDamage(float getDamageAmount)
    {
        character.currentHp -= getDamageAmount;
        if (character.currentHp <= 0)
        {
            character.currentHp = 0;
            currentState = TurnState.DEAD;
        }
        if (character.currentHp >= character.maxHp)
        {
            character.currentHp = character.maxHp;
        }

        UpdateAllyPanel();
    }

    IEnumerator DoDamage()
    {
        cam.transform.DOMove(new Vector2(attackTarget.transform.position.x, attackTarget.transform.position.y + 1), .5f);
        cam.ZoomIn();
        yield return new WaitForSeconds(.5f);
        BaseAttack chosenAttack = battleStateMachine.PerformList[0].chosenAttack;

        int patterns = 0;
        float sizeX = 0;
        float damage = 0;
        switch (chosenAttack.target)
        {
            case Target.ENEMY:
                EnemyStateMachine enemyStateMachine = attackTarget.transform.GetChild(0).GetComponent<EnemyStateMachine>();

                attackCanvas.SetActive(true);

                if (enemyStateMachine.enemy.pattern.Count < 16 && chosenAttack.pattern.Count < 16)
                {
                    background.sizeDelta = new Vector2(180, 5);
                }
                else
                {
                    background.sizeDelta = new Vector2(360, 5);
                }
                sizeX = background.sizeDelta.x - 20;

                for (int i = 0; i < enemyStateMachine.enemy.pattern.Count; i++)
                {
                    if (enemyStateMachine.enemy.pattern[i])
                    {
                        objectPool.SpawnUI("Note", upperNotes, new Vector2((-sizeX / 2) + (sizeX / enemyStateMachine.enemy.pattern.Count) * i, 0), "Enemy");
                        patterns++;
                    }
                }
                for (int i = 0; i < chosenAttack.pattern.Count; i++)
                {
                    if (chosenAttack.pattern[i])
                    {
                        objectPool.SpawnUI("Note", belowNotes, new Vector2((-sizeX / 2) + (sizeX / chosenAttack.pattern.Count) * i, 0), "Character");
                        patterns++;
                    }
                }
                lineManager.LineMove();
                yield return new WaitForSeconds(4f);

                damage = (character.attack + chosenAttack.attackDamage) * lineManager.score / patterns;
                Debug.Log("(" + character.attack + " + " + chosenAttack.attackDamage + ") * " + lineManager.score + " / " + patterns + " = " + damage);
                enemyStateMachine.TakeDamage(damage);
                break;
            case Target.ALLY:
                CharacterStateMachine characterStateMachine = attackTarget.transform.GetChild(0).GetComponent<CharacterStateMachine>();

                attackCanvas.SetActive(true);

                if (characterStateMachine.character.pattern.Count < 16 && chosenAttack.pattern.Count < 16)
                {
                    background.sizeDelta = new Vector2(180, 5);
                }
                else
                {
                    background.sizeDelta = new Vector2(360, 5);
                }
                sizeX = background.sizeDelta.x - 20;

                for (int i = 0; i < characterStateMachine.character.pattern.Count; i++)
                {
                    if (characterStateMachine.character.pattern[i])
                    {
                        objectPool.SpawnUI("Note", upperNotes, new Vector2((-sizeX / 2) + (sizeX / characterStateMachine.character.pattern.Count) * i, 0), "Enemy");
                        patterns++;
                    }
                }
                for (int i = 0; i < chosenAttack.pattern.Count; i++)
                {
                    if (chosenAttack.pattern[i])
                    {
                        objectPool.SpawnUI("Note", belowNotes, new Vector2((-sizeX / 2) + (sizeX / chosenAttack.pattern.Count) * i, 0), "Character");
                        patterns++;
                    }
                }
                lineManager.LineMove();
                yield return new WaitForSeconds(4f);

                damage = (- character.attack + chosenAttack.attackDamage) * lineManager.score / patterns;
                Debug.Log("(" + character.attack + " + " + chosenAttack.attackDamage + ") * " + lineManager.score + " / " + patterns + " = " + damage);
                characterStateMachine.TakeDamage(damage);
                break;
        }
        attackCanvas.SetActive(false);

        cam.transform.DOMove(Vector2.zero, .5f);
        cam.ZoomOut();
    }

    void CreateAllyPanel()
    {
        currentState = TurnState.PROCESSING;

        allyPanel = ObjectPool.instance.Spawn("AllyPanel");
        stats = allyPanel.GetComponent<AllyPanelStats>();
        stats.allyName.text = character.name;
        stats.allyHp.text = character.currentHp.ToString();
        stats.allyMp.text = character.currentMp.ToString();
        stats.allyMaxHp.text = "/ " + character.maxHp.ToString();
        stats.allyMaxMp.text = "/ " + character.maxMp.ToString();

        progressBar = stats.progressBar;
        allyPanel.transform.SetParent(panelSpacer, false);
    }

    void UpdateAllyPanel()
    {
        stats.allyHp.text = character.currentHp.ToString();
        stats.allyMp.text = character.currentMp.ToString();
    }

    public void LevelUp()
    {
        character.level++;
        character.exp -= 100;
        character.maxHp += character.levelHp;
        character.currentHp += character.levelHp;
        character.maxMp += character.levelMp;
        character.currentMp += character.levelMp;
        character.attack += character.levelAttack;
        character.armor += character.levelArmor;
    }
}
