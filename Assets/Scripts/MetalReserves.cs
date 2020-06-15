using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MetalReserves : MonoBehaviour
{

    public float reserveLevel;
    public GameObject beadTemplate;
    float beadSize = 15;
    RectTransform rectTransform;
    public List<BeadHelper> beads;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
        beads = new List<BeadHelper>();
        reserveLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBeads(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddBead();
        }
    }

    public void AddBead()
    {
        reserveLevel += beadSize;

        Bounds spawnBounds = GetComponent<PolygonCollider2D>().bounds;

        Vector2 spawnCenter = (Vector2) spawnBounds.center + Vector2.up * spawnBounds.size.y / 4;
        Vector2 spawn = spawnCenter + new Vector2(Random.Range(-1.0f, 1.0f) * spawnBounds.size.x / 8, Random.Range(-1.0f, 1.0f) * spawnBounds.size.y / 8);

        GameObject newBead = Instantiate(beadTemplate, new Vector2(0.0f,0.0f), Quaternion.identity);
        newBead.transform.SetParent(transform);
        newBead.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        newBead.transform.position = spawn;

        newBead.GetComponent<BeadHelper>().maxSize = beadSize;
        beads.Add(newBead.GetComponent<BeadHelper>());
    }

    public void DecreaseReserves(float amount)
    {
        reserveLevel -= amount;

        float amount_left = amount;

        while (amount_left > 0)
        {
            amount_left -= beads[0].decrement(amount_left);
            if (beads[0].size == 0) beads.Remove(beads[0]);
        }
    }
}
