using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContextSprite : MonoBehaviour
{
    public Sprite keyboardSprite;
    public Sprite gamepadSprite;

    SpriteRenderer spriteRenderer;
    bool gamepadEnabled;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gamepadEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamepadEnabled != (PlayerPrefs.GetInt("InputMode",0) == 1))
        {
            gamepadEnabled = PlayerPrefs.GetInt("InputMode", 0) == 1;

            if (gamepadEnabled)
            {
                spriteRenderer.sprite = gamepadSprite;
            }
            else
            {
                spriteRenderer.sprite = keyboardSprite;
            }
        }
    }
}
