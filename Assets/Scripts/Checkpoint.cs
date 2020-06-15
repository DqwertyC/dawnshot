using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Checkpoint : MonoBehaviour
{
    bool active;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    SpriteRenderer checkpointRenderer;


    // Start is called before the first frame update
    void Start()
    {
        checkpointRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        active = true;
        checkpointRenderer.sprite = activeSprite;

    }

    public void Deactivate()
    {
        active = false;
        checkpointRenderer.sprite = inactiveSprite;
    }
}
