using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();
    [SerializeField]
    List<Sprite> spritesDefault = new List<Sprite>();
    string spriteName;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -transform.GetSiblingIndex();

        spriteName = this.name;
        sprites = SpriteData.instance.sprites[spriteName];
    }

    private void LateUpdate()
    {
        if (spriteRenderer.sprite.Equals(spritesDefault[0]))
            spriteRenderer.sprite = sprites[0];
        if (spriteRenderer.sprite.Equals(spritesDefault[1]))
            spriteRenderer.sprite = sprites[1];
        if (spriteRenderer.sprite.Equals(spritesDefault[2]))
            spriteRenderer.sprite = sprites[2];
        if (spriteRenderer.sprite.Equals(spritesDefault[3]))
            spriteRenderer.sprite = sprites[3];
    }
}
