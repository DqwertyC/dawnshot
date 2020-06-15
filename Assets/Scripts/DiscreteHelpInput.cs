using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscreteHeldInput
{
    float minHoldTime;
    float stepHoldTime;

    float oldValue = 0.0f;
    float heldTime = 0.0f;
    bool overMinTime = false;

    public DiscreteHeldInput(float minTime, float stepTime)
    {
        minHoldTime = minTime;
        stepHoldTime = stepTime;
    }

    public int Update(float value)
    {
        int retval = 0;

        if (Mathf.Abs(value) > 0.1f)
        {
            heldTime += Time.deltaTime;

            if (Mathf.Abs(oldValue) < 0.1f)
            {
                retval = (int)Mathf.Sign(value);
            }
            else
            {
                if (!overMinTime && heldTime >= minHoldTime)
                {
                    heldTime = 0;
                    overMinTime = true;
                    retval = (int)Mathf.Sign(value);
                }
                else if (overMinTime && heldTime >= stepHoldTime)
                {
                    heldTime = 0;
                    retval = (int)Mathf.Sign(value);
                }
            }
        }
        else
        {
            heldTime = 0;
            overMinTime = false;
        }

        oldValue = value;
        return retval;
    }
}
