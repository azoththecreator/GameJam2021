using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyStateMachine : MonoBehaviour
{
    public BattleStateMachine battleStateMachine;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;

    private float currentCooldown = 0f, maxCooldown = 5;
    public GameObject selector;

    public bool actionStarted = false;
    public GameObject attackTarget;

    bool alive = true;

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
        gameObject.tag = "Enemy";
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;

        GetComponent<Image>().sprite = enemy.sprite;
        GetComponent<Image>().SetNativeSize();
    }

    public void DataLoad(BaseEnemy enemyData)
    {
        currentCooldown = 0;

        enemy.name = enemyData.name;
        enemy.sprite = enemyData.sprite;
        enemy.maxHp = enemyData.maxHp;
        enemy.currentHp = enemy.maxHp;
        enemy.speed = enemyData.speed;
        enemy.attack = enemyData.attack;
        enemy.armor = enemyData.armor;
        enemy.EnemyType = enemyData.EnemyType;
        enemy.attacks = enemyData.attacks;
        enemy.level = enemyData.level;
        enemy.exp= enemyData.exp;
        enemy.pattern = enemyData.pattern;
    }

    private void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpdateProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
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
                    gameObject.tag = "DeadEnemy";
                    battleStateMachine.EnemiesInBattle.Remove(transform.parent.gameObject);
                    selector.SetActive(false);

                    for (int i = 0; i < battleStateMachine.CharactersInBattle.Count; i++)
                    {
                        CharacterStateMachine characterStateMachine = battleStateMachine.CharactersInBattle[i].transform.GetChild(0).GetComponent<CharacterStateMachine>();
                        BaseCharacter character = characterStateMachine.character;
                        if (character.currentHp > 0)
                        {
                            if (character.level < enemy.level)
                                character.exp += (enemy.level - character.level) * enemy.exp;
                            if (character.exp >= 100)
                            {
                                characterStateMachine.LevelUp();
                            }
                        }
                    }

                    if (battleStateMachine.EnemiesInBattle.Count > 0)
                    {
                        for (int i = 0; i < battleStateMachine.PerformList.Count; i++)
                        {
                            if (battleStateMachine.PerformList[i].AttacksGameObject.Equals(transform.parent.gameObject))
                            {
                                battleStateMachine.PerformList.Remove(battleStateMachine.PerformList[i]);
                            }
                            if (battleStateMachine.PerformList[i].AttackersTarget.Equals(gameObject))
                            {
                                battleStateMachine.PerformList[i].AttackersTarget = battleStateMachine.EnemiesInBattle[Random.Range(0, battleStateMachine.EnemiesInBattle.Count)];
                            }
                        }
                    }
                    //색바꾸기

                    alive = false;
                    battleStateMachine.EnemyButtons();
                    battleStateMachine.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
                break;
        }
    }

    void UpdateProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime * enemy.speed * .2f * battleStateMachine.timePause;

        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.CHOOSEACTION;
            battleStateMachine.timePause = 0;
        }
    }

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.name;
        myAttack.Type = "Enemy";
        myAttack.AttacksGameObject = gameObject;

        do
        {
            int num = Random.Range(0, enemy.attacks.Count);
            myAttack.chosenAttack = enemy.attacks[num];
        }
        while (myAttack.chosenAttack.target.Equals(Target.ALLY) && battleStateMachine.EnemiesInBattle.Count < 2);

        switch (myAttack.chosenAttack.target)
        {
            case Target.ENEMY:
                myAttack.AttackersTarget = battleStateMachine.CharactersInBattle[Random.Range(0, battleStateMachine.CharactersInBattle.Count)];
                break;
            case Target.ALLY:
                myAttack.AttackersTarget = battleStateMachine.EnemiesInBattle[Random.Range(0, battleStateMachine.EnemiesInBattle.Count)];
                break;
        }
        battleStateMachine.CollectActions(myAttack);
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
        battleStateMachine.battleStates = BattleStateMachine.PerformAction.WAIT;

        actionStarted = false;
        currentCooldown = 0f;
        currentState = TurnState.PROCESSING;
        battleStateMachine.timePause = 1;
    }

    IEnumerator DoDamage()
    {
        cam.transform.DOMove(new Vector2(attackTarget.transform.position.x, attackTarget.transform.position.y + 1), .5f);
        cam.ZoomIn();
        yield return new WaitForSeconds(.5f);

        BaseAttack chosenAttack = battleStateMachine.PerformList[0].chosenAttack;
        float damage = 0;
        switch (chosenAttack.target)
        {
            case Target.ENEMY:
                CharacterStateMachine characterStateMachine = attackTarget.transform.GetChild(0).GetComponent<CharacterStateMachine>();
                attackCanvas.SetActive(true);
                int patterns = 0;

                if (characterStateMachine.character.pattern.Count < 16 && chosenAttack.pattern.Count < 16)
                {
                    background.sizeDelta = new Vector2(180, 5);
                }
                else
                {
                    background.sizeDelta = new Vector2(360, 5);
                }
                float sizeX = background.sizeDelta.x - 20;

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
                damage = (enemy.attack + chosenAttack.attackDamage);
                if (!lineManager.score.Equals(0))
                    damage *= 1 / (lineManager.score / patterns);
                else
                    damage *= 2;

                Debug.Log(damage);
                attackTarget.transform.GetChild(0).GetComponent<CharacterStateMachine>().TakeDamage(damage);
                attackCanvas.SetActive(false);
                break;
            case Target.ALLY:
                damage = chosenAttack.attackDamage;
                yield return new WaitForSeconds(1f);
                attackTarget.transform.GetChild(0).GetComponent<EnemyStateMachine>().TakeDamage(damage);
                break;
        }
        cam.transform.DOMove(Vector2.zero, .5f);
        cam.ZoomOut();
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.currentHp -= getDamageAmount;
        if (enemy.currentHp <= 0)
        {
            enemy.currentHp = 0;
            currentState = TurnState.DEAD;
        }
    }
}
