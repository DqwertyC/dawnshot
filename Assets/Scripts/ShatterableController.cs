using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShatterableController : MonoBehaviour
{
    public GameObject shattered;
    public bool shatter = false;

    public Vector2 staticLoad;
    public Vector2 dynamicLoad;

    public int coinBreakInertia = 50;
    public int waxBreakInertia = 100000;

    void Update()
    {
        if (shatter) Shatter();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collidingBody = collision.collider.GetComponentInParent<Rigidbody2D>();

        if (collidingBody != null)
        {
            Collider2D collidingCollider = collidingBody.GetComponent<Collider2D>();

            float area = 1.0f;

            if (collidingCollider != null)
            {
                area = Mathf.Pow(collidingCollider.bounds.size.x * collidingCollider.bounds.size.y,3.0f);
            }

            int load = (int) (collidingBody.mass * collision.relativeVelocity.magnitude) / 10;
            load *= 10;

            if (collidingBody.GetComponent<MetalObject>() && load > coinBreakInertia) Shatter();
            else if (collidingBody.GetComponent<WaxController>() && load > waxBreakInertia) Shatter();
        }
    }

    public void Shatter()
    {
        Vector2 scale = GetComponent<SpriteRenderer>().size;
        GameObject newObject = Instantiate(shattered, transform.position, transform.rotation);

        for (int i = 0; i < newObject.transform.childCount; i++)
        {
            Transform child = newObject.transform.GetChild(i);
            child.GetComponent<SpriteRenderer>().size = scale;
            child.localPosition = child.localPosition * scale;
        }

        Destroy(gameObject);
    }
}
