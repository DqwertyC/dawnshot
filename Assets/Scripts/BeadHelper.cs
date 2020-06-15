using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BeadHelper : MonoBehaviour
{
    RectTransform rectTransform;
    public float size;
    public float maxSize;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        size = (int) Mathf.Floor(maxSize);
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale,new Vector3(size / maxSize, size / maxSize, 1.0f), 0.1f);
        if (rectTransform.localScale.x < 0.01f) Destroy(this);
    }

    public float decrement(float amount)
    {
        float amount_removed = Mathf.Min(amount, size);

        if (size >= amount)
        {
            size -= amount;
        }
        else
        {
            size = 0;
        }

        return amount_removed;
    }
}
