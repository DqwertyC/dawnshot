using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public class SecretFader : MonoBehaviour
{
    public LayerMask revealsHidden;

    Tilemap tilemap;
    TilemapCollider2D tilemapCollider;
    ContactFilter2D collisionFilter;
    float alpha;
    float targetAlpha;
    float alphaDamp;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapCollider = GetComponent<TilemapCollider2D>();

        collisionFilter = new ContactFilter2D();
        collisionFilter.SetLayerMask(revealsHidden);

        alpha = 1;
        targetAlpha = 1;
        alphaDamp = 0;
    } 

    // Update is called once per frame
    void Update()
    {
        Collider2D[] collisions = new Collider2D[4];
        tilemapCollider.OverlapCollider(collisionFilter, collisions);

        targetAlpha = 1;

        for (int i = 0; i < collisions.Length; i++)
        {
            if (collisions[i] != null)
            {
                targetAlpha = 0;
            }
        }

        alpha = Mathf.SmoothDamp(alpha, targetAlpha, ref alphaDamp, 0.5f);
        tilemap.color = new Color(tilemap.color.r,tilemap.color.g,tilemap.color.b, alpha);
    }
}
