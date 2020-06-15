using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    public Vector2 startpos;
    public GameObject cam;
    public float parallaxEffect;
    public bool followVertical;
    public bool fadeVertical;
    public float startHeight;
    public float endHeight;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startpos.x + distance, followVertical ? transform.position.y : startpos.y, transform.position.z);

        if (temp > startpos.x + length) startpos.x += length;
        else if (temp < startpos.x - length) startpos.x -= length;

        if (fadeVertical)
        {
            SpriteRenderer thisImage = GetComponent<SpriteRenderer>();

            float height = transform.position.y;

            if (transform.position.y < startHeight)
            {
                thisImage.color = thisImage.color.SetAlpha(0.0f);
            }
            else if (transform.position.y > endHeight)
            {
                thisImage.color = thisImage.color.SetAlpha(1.0f);
            }
            else
            {
                thisImage.color = thisImage.color.SetAlpha((height - startHeight) / (endHeight - startHeight));
            }
        }

    }
}
