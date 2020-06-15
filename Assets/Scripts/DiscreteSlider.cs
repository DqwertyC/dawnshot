using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteSlider : MonoBehaviour
{
    public int level;

    public GameObject[] posObjects;
    public GameObject[] negObjects;

    // Start is called before the first frame update
    void Start()
    {
        setLevel(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setLevel(int level)
    {
        this.level = level;

        if (level > 0)
        {
            foreach (GameObject o in negObjects)
            {
                o.SetActive(false);
            }

            for (int i = 0; i < posObjects.Length; i++)
            {
                if ((i + 1) <= level) posObjects[i].SetActive(true);
                else posObjects[i].SetActive(false);
            }
        }
        else if (level < 0)
        {
            foreach (GameObject o in posObjects)
            {
                o.SetActive(false);
            }

            for (int i = 0; i < negObjects.Length; i++)
            {
                if ((i + 1) <= -level) negObjects[i].SetActive(true);
                else negObjects[i].SetActive(false);
            }
        }
        else
        {
            foreach (GameObject o in negObjects)
            {
                o.SetActive(false);
            }

            foreach (GameObject o in posObjects)
            {
                o.SetActive(false);
            }
        }
    }
}
