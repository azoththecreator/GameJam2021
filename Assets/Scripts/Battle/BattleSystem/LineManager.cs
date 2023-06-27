using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineManager : MonoBehaviour
{
    public float score = 0, scoreToGet = 0;
    bool isNearEnemy = false, isNearSkill = false;
    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    Transform upperNotes, belowNotes;
    string enemyTag = "Enemy", characterTag = "Character";
    string bad = "bad", good = "good", perfect = "perfect";

    [SerializeField]
    RectTransform background;

    private void OnDisable()
    {
        score = 0;

        for (int i = 0; i < upperNotes.childCount; i++)
        {
            upperNotes.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < belowNotes.childCount; i++)
        {
            belowNotes.GetChild(i).gameObject.SetActive(false);
        }
    }


    public void LineMove()
    {
        rectTransform.DOLocalMoveX(-background.rect.width / 2 - 10, 0);
        rectTransform.DOLocalMoveX(background.rect.width + 20, 4).SetEase(Ease.Linear).SetRelative();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        { 
            if (isNearEnemy)
            {
                isNearEnemy = false;
                score += scoreToGet;
                Debug.Log(scoreToGet);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isNearSkill)
            {
                isNearSkill = false;
                score += scoreToGet;
                Debug.Log(scoreToGet);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.CompareTag(enemyTag))
        {
            if (collision.name.Equals(bad))
            {
                isNearEnemy = true;
                scoreToGet = .5f;
            }
            if (collision.name.Equals(good))
            {
                scoreToGet = 1;
            }
            if (collision.name.Equals(perfect))
            {
                scoreToGet = 2;
            }
        }
        if (collision.transform.parent.CompareTag(characterTag))
        {
            if (collision.name.Equals(bad))
            {
                isNearSkill = true;
                scoreToGet = .5f;
            }
            if (collision.name.Equals(good))
            {
                scoreToGet = 1;
            }
            if (collision.name.Equals(perfect))
            {
                scoreToGet = 2;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent.CompareTag(enemyTag))
        {
            if (collision.name.Equals(perfect))
            {
                scoreToGet = 1;
            }
            if (collision.name.Equals(good))
            {
                scoreToGet = .5f;
            }
            if (collision.name.Equals(bad))
            {
                scoreToGet = 0;
                isNearEnemy = false;
            }
        }
        if (collision.transform.parent.CompareTag(characterTag))
        {
            if (collision.name.Equals(perfect))
            {
                scoreToGet = 1;
            }
            if (collision.name.Equals(good))
            {
                scoreToGet = .5f;
            }
            if (collision.name.Equals(bad))
            {
                scoreToGet = 0;
                isNearSkill = false;
            }
        }
    }
}
