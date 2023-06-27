using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Heal : BaseAttack
{
    public Attack_Heal()
    {
        attackName = "Heal";
        attackDescription = "Heal an ally";
        attackDamage = -5;
        attackCost = 5;
    }
}
