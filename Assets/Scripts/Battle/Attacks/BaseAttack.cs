using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Target
{
    ENEMY,
    ENEMYALL,
    ALLY,
    ALLYALL
}

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;
    public string attackDescription;

    public float attackDamage;
    public float attackCost;

    public List<bool> pattern = new List<bool>();
    public Target target;
}
