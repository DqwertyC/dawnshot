using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHover : MonoBehaviour
{
    public float initTime = 0;
    float elapsedTime;
    Vector3 posInit;


    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = initTime;
        posInit = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += 2 * Time.deltaTime;
        elapsedTime %= 2 * Mathf.PI;

        transform.position = posInit +  0.5f * Vector3.up * Mathf.Sin(elapsedTime);


    }
}
