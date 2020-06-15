using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinks : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector2.Lerp(transform.localScale, Vector3.zero, 0.01f);
        if (transform.localScale.x < 0.05f) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (transform.parent != null) // if object has a parent
        {
            if (transform.childCount <= 1) // if this object is the last child
            {
                Destroy(transform.parent.gameObject, 0.1f); // destroy parent a few frames later
            }
        }
    }
}
