using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Rigidbody2D rigidbody;
    public float speed;
    float jumpPower = 2;
    bool canJump = true;

    SpriteRenderer spriteRenderer;
    Animator animator;
    string isMoving = "isMoving", isJumping = "isJumping";

    bool inField;
    string tagField = "Field";
    float encounterPercentage = 1;
    BattleStateMachine battleStateMachine;
    public float timePause = 1;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        battleStateMachine = GameObject.Find("GameManager").GetComponent<BattleStateMachine>();
        List<GameObject> characterDB = battleStateMachine.GetComponent<CharacterDataBase>().characterDB;

        for (int i = 0; i < characterDB.Count; i++)
        {
            if (characterDB[i].name.Equals(this.name))
            {
                Instantiate(characterDB[i], transform);
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            if (canJump && !inField)
            {
                canJump = false;
                animator.SetBool(isJumping, !canJump);
                rigidbody.AddForce(Vector2.up * jumpPower * timePause, ForceMode2D.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        speed = Input.GetAxisRaw("Horizontal");
        if (inField)
            speed *= .5f * timePause;
        rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);

        if (speed < 0 && !spriteRenderer.flipX)
            spriteRenderer.flipX = true;
        else if (speed > 0 && spriteRenderer.flipX)
            spriteRenderer.flipX = false;

        animator.SetBool(isMoving, speed != 0);
        
        if (animator.speed != Mathf.Abs(speed))
            animator.speed = Mathf.Abs(speed);

        if (rigidbody.velocity.y.Equals(0))
        {
            if (!canJump)
            {
                canJump = true;
                animator.SetBool(isJumping, !canJump);
            }
        }
        else
        {
            if (canJump)
            {
                canJump = false;
                animator.SetBool(isJumping, !canJump);
            }
        }

        if (!rigidbody.velocity.x.Equals(0) && inField)
        {
            int encounter = Random.Range(0, 100);
            if (encounter < encounterPercentage)
            {
                List<CharacterStateMachine> partyMembers = new List<CharacterStateMachine>();
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    partyMembers.Add(transform.parent.GetChild(i).GetChild(0).GetComponent<CharacterStateMachine>());
                }
                battleStateMachine.PartyMembers = partyMembers;
                battleStateMachine.BattleStart();
                timePause = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagField))
        {
            inField = true;
            battleStateMachine.fieldData = collision.GetComponent<FieldData>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagField))
        {
            inField = false;
            battleStateMachine.fieldData = null;
        }
    }

}
