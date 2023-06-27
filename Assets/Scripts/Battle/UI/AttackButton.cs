using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack attackToPerform;

    public void Cast()
    {
        GameObject.Find("GameManager").GetComponent<BattleStateMachine>().Input4(attackToPerform);
    }
}
