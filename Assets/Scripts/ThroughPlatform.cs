using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class ThroughPlatform : MonoBehaviour
{
    PlatformEffector2D platform;
    float oldArc;
    bool passingThrough = false;

    void Start()
    {
        platform = GetComponent<PlatformEffector2D>();
        oldArc = platform.surfaceArc;
    }

    public void PassThrough(float time)
    {
        platform.surfaceArc = 0;
        if (!passingThrough)
        {
            passingThrough = true;
            Invoke("ResetPlatform", time);
        }
        
    }

    void ResetPlatform()
    {
        passingThrough = false;
        platform.surfaceArc = oldArc;
    }
}
