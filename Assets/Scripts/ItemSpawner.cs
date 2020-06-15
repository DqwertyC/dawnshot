using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    public GameObject objectTemplate;
    private GameObject latest;

    // Start is called before the first frame update
    void Start()
    {
        SpawnItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (latest == null)
        {
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        latest = Instantiate(objectTemplate, transform.position, Quaternion.identity);
    }
}
