using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MetalObject : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rigidBody;
    Collider2D objectCollider;

    private AudioSource audioPlayer;

    public SteelLine line;

    WaxController target;
    Collider2D lastAnchor;
    Color mainColor;

    bool inRadius = false;
    bool selected = false;

    float maxRadius;
    float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
        audioPlayer = GetComponent<AudioSource>();
        lastAnchor = null;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            line.UpdateLine(this.transform.position, target.transform.position, inRadius, maxRadius, timeLeft, selected);
        }
        else
        {
            line.UpdateLine(this.transform.position, (Vector2)this.transform.position + Vector2.up, false, 1.0f, 1.0f, false);
        }
        

        inRadius = false;
        selected = false;
    }

    public void SetInRadius(WaxController target, float maxRadius, float timeLeft, bool active)
    {
        this.target = target;

        if (active)
        {
            this.inRadius = true;
            this.maxRadius = maxRadius;
            this.timeLeft = timeLeft;
        }
    }

    public void SetAsSelected()
    {
        selected = true;
    }

    public float Anchored(Vector2 forceDir)
    {
        float anchored = 0.0f;
        lastAnchor = null;

        if (rigidBody == null || rigidBody.bodyType == RigidbodyType2D.Static) anchored = 1.0f;
        else
        {
            RaycastHit2D[] collisions = null;
            RaycastHit2D nearestObstacle = new RaycastHit2D();
            bool foundObstacle = false;
            float nearestDistance = 1.0f;

            Vector2 edgePosition = transform.position;

            collisions = Physics2D.RaycastAll((Vector2) transform.position + forceDir.normalized, -forceDir.normalized, 1.0f, LayerMask.GetMask("Metal"));

            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].collider == this.objectCollider)
                {
                    edgePosition = collisions[i].point;
                }
            }

            collisions = Physics2D.RaycastAll(edgePosition, forceDir.normalized, 0.1f, LayerMask.GetMask("Metal","Obstacles"));
            
            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].collider != this.objectCollider)
                {
                    if (collisions[i].distance < nearestDistance)
                    {
                        foundObstacle = true;
                        nearestObstacle = collisions[i];
                        nearestDistance = nearestObstacle.distance;
                    }
                }
            }

            if (foundObstacle)
            {
                MetalObject touching = nearestObstacle.collider.GetComponentInParent<MetalObject>();

                if (touching != null)
                {
                    anchored = touching.Anchored(forceDir);
                }
                else
                {
                    anchored = Mathf.Sin(Mathf.Deg2Rad * (90.0f - Vector2.Angle(nearestObstacle.normal, -forceDir)));         
                }

                lastAnchor = nearestObstacle.collider;
            }

        }


        return anchored;
    }

    public void TransferForce(Vector2 force)
    {
        if (lastAnchor != null)
        {
            Rigidbody2D anchorItem = lastAnchor.GetComponentInParent<Rigidbody2D>();

            if (anchorItem != null)
            {
                anchorItem.AddForce(force, ForceMode2D.Force);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (audioPlayer != null && target != null)
        {
            float impact = Vector2.Dot(collision.relativeVelocity, collision.GetContact(0).normal);
            float volume = 4 / Mathf.Max((transform.position - target.transform.position).magnitude,4);
            if (impact > 2)
            {
                audioPlayer.pitch = Random.Range(0.9f,1.45f);
                audioPlayer.volume = volume;
                audioPlayer.Play();
            }
        }
    }
}
