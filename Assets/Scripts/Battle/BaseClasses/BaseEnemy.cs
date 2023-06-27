using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{
    public enum Type
    {
        NULL,
        EARTH,
        WATER,
        FIRE,
        DARK
    }

    public Type EnemyType;

    public List<BaseAttack> attacks = new List<BaseAttack>();
}
