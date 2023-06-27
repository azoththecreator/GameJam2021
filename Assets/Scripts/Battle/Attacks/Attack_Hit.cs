using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Hit : BaseAttack
{
    public Attack_Hit()
    {
        attackName = "Hit";
        attackDescription = "Hit enemy with a stick";
        attackDamage = 5;
        attackCost = 0;
    }
}
