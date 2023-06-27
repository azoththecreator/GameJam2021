using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataBase : MonoBehaviour
{
    public List<GameObject> characterDB;

    private void Awake()
    {
        object[] data = Resources.LoadAll("Prefabs/Characters");
        foreach (object character in data)
        {
            characterDB.Add(character as GameObject);
        }
    }
}
