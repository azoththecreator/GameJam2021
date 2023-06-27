using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseCharacter : BaseClass
{
    public float maxMp, currentMp;

    public int levelHp, levelMp, levelAttack, levelArmor;

    public List<BaseAttack> skills = new List<BaseAttack>();
    public List<AvailableAttacks> skillsLearn = new List<AvailableAttacks>();
}
