using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseClass
{
    public string name;
    public Sprite sprite;

    public float maxHp, currentHp;

    public float speed, attack, armor;

    public int level, exp;

    public List<bool> pattern = new List<bool>();
}
