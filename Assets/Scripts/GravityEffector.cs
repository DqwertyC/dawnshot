using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GravityEffector : MonoBehaviour
{
    public float gravScale = 1.0f;

    private Hashtable storedColliders;
    private Collider2D thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider2D>();
        storedColliders = new Hashtable();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] touching = Physics2D.OverlapBoxAll(thisCollider.bounds.center, thisCollider.bounds.size, 0.0f);

        //Get Current Colliders
        HashSet<Rigidbody2D> currentColliders = new HashSet<Rigidbody2D>();
        for (int i = 0; i < touching.Length; i++)
        {
            Rigidbody2D collidingBody = touching[i].attachedRigidbody;
            if (collidingBody != null) currentColliders.Add(collidingBody);
        }

        //Get list of no-longer current colliders
        HashSet<Rigidbody2D> removedColliders = new HashSet<Rigidbody2D>();
        foreach (Object o in storedColliders.Keys)
        {
            if (!currentColliders.Contains((Rigidbody2D)o))
                removedColliders.Add((Rigidbody2D)o);
        }

        //Get list of new colliders
        HashSet<Rigidbody2D> newColliders = new HashSet<Rigidbody2D>();
        foreach (Rigidbody2D r in currentColliders)
        {
            if (!storedColliders.ContainsKey(r))
                newColliders.Add(r);
        }

        //Remove old colliders
        foreach (Rigidbody2D r in removedColliders)
        {
            if (r != null)
            {
                r.gravityScale = (float)storedColliders[r];
                storedColliders.Remove(r);
            }
        }

        //Add new colliders
        foreach (Rigidbody2D r in newColliders)
        {
            storedColliders.Add(r, r.gravityScale);
            r.gravityScale = gravScale;
        }
    }
}
