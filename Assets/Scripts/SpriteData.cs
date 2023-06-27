using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteData : MonoBehaviour
{
    public static SpriteData instance = null;
    public Dictionary<string, List<Sprite>> sprites;

    [SerializeField]
    List<Sprite> boy = new List<Sprite>();
    [SerializeField]
    List<Sprite> girl = new List<Sprite>();
    [SerializeField]
    List<Sprite> rogue = new List<Sprite>();


    private void Awake()
    {
        instance = this;

        sprites = new Dictionary<string, List<Sprite>>();
        sprites.Add("boy", boy);
        sprites.Add("girl", girl);
        sprites.Add("rogue", rogue);
    }
}
