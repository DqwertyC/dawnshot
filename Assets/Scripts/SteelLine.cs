using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SteelLine : MonoBehaviour
{
    bool lineEnabled;
    bool lineSelected;
    float maxLen;
    float timeLeft;

    float alpha;

    Vector2 anchorA;
    Vector2 anchorB;

    Color colSelect = Color.cyan;
    Color colNormal = Color.blue;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        anchorA = (Vector2)this.transform.position + Vector2.up;
        anchorB = (Vector2)this.transform.position + Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (lineEnabled)
        {
            Vector2 center = (anchorA + anchorB) / 2;
            Vector2 length = anchorB - anchorA;

            float scale = length.magnitude;
            Quaternion angle = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, length));

            transform.localScale = new Vector3(scale, 1, 1);
            transform.SetPositionAndRotation(center, angle);
            spriteRenderer.sortingOrder = lineSelected ? 2 : 1;

            alpha = 1 - Mathf.Pow(scale / maxLen,2);
            if (timeLeft < 2.0f) alpha *= 0.75f + 0.25f * Mathf.Sin(8 * Mathf.PI * Time.time);
            else if (timeLeft < 8.0f) alpha *= 0.75f + 0.25f * Mathf.Sin(2 * Mathf.PI * Time.time);

            spriteRenderer.color = lineSelected ? ColorWithAlpha(colSelect,alpha) : ColorWithAlpha(colNormal,alpha);
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    public void UpdateLine(Vector2 start, Vector2 end, bool enabled, float maxLen, float timeLeft, bool selected)
    {
        this.anchorA = start;
        this.anchorB = end;
        this.lineEnabled = enabled;
        this.lineSelected = selected;
        this.maxLen = maxLen;
        this.timeLeft = timeLeft;
    }

    private Color ColorWithAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
