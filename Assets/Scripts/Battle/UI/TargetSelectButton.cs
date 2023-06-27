using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectButton : MonoBehaviour
{
    public GameObject targetPrefab;

    public void SelectEnemy()
    {
        GameObject.Find("GameManager").GetComponent<BattleStateMachine>().Input2(targetPrefab);
    }

    public void HideSelector()
    {
        targetPrefab.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }
    public void ShowSelector()
    {
        targetPrefab.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
}
