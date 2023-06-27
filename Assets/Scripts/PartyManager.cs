using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    Transform leader, prevMember;
    Rigidbody2D leaderRigidbody;
    PlayerManager playerManager;

    int steps;
    Queue<float> recordX = new Queue<float>();
    Queue<float> recordY = new Queue<float>();

    SpriteRenderer spriteRenderer;
    Animator animator;
    float lastPosX, lastPosY;
    int accurate = 1000;

    private void Start()
    {
        leader = transform.parent.GetChild(0);
        prevMember = transform.parent.GetChild(transform.GetSiblingIndex() - 1);
        leaderRigidbody = leader.GetComponent<Rigidbody2D>();
        playerManager = leader.GetComponent<PlayerManager>();

        steps = transform.GetSiblingIndex() * 5;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lastPosX = transform.position.x;
        lastPosY = transform.position.y;

        List<GameObject> characterDB = GameObject.Find("GameManager").GetComponent<CharacterDataBase>().characterDB;

        for (int i = 0; i < characterDB.Count; i++)
        {
            if (characterDB[i].name.Equals(this.name))
            {
                Instantiate(characterDB[i], transform);
                break;
            }
        }
    }

    void FixedUpdate()
    {
        float currentX = Mathf.Round(transform.position.x * accurate);
        float currentY = Mathf.Round(transform.position.y * accurate);

        if (leaderRigidbody.velocity.x != 0)
           recordX.Enqueue(leader.position.x);
        recordY.Enqueue(leader.position.y);

        if (recordX.Count > steps)
            transform.position = new Vector2(recordX.Dequeue(), transform.position.y);

        if (recordY.Count > steps)
            transform.position = new Vector2(transform.position.x, recordY.Dequeue());

        if (lastPosX > currentX || (lastPosX.Equals(currentX) && transform.position.x > prevMember.position.x))
        {
            if (!spriteRenderer.flipX)
                spriteRenderer.flipX = true;
        }
        else if (lastPosX < currentX || (lastPosX.Equals(currentX) && transform.position.x < prevMember.position.x))
        {
            if (spriteRenderer.flipX)
                spriteRenderer.flipX = false;
        }

        animator.SetInteger("isMoving", Mathf.CeilToInt(Mathf.Abs(lastPosX - currentX)));
        animator.SetInteger("isJumping", Mathf.CeilToInt(Mathf.Abs(lastPosY - currentY)));

        if (animator.speed != Mathf.Abs(playerManager.speed))
            animator.speed = Mathf.Abs(playerManager.speed);

        lastPosX = currentX;
        lastPosY = currentY;
    }
}
