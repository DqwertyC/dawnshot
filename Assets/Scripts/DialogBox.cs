using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class DialogBox : MonoBehaviour
{
    public TextMeshProUGUI dialogDisplay;
    Canvas dialogCanvas;

    public string[] dialogScreens;
    int activeScreenIndex;

    Collider2D bounds;
    bool triggered;
    bool active;

    bool lastOkayPressed;

    // Start is called before the first frame update
    void Start()
    {
        bounds = GetComponent<Collider2D>();
        activeScreenIndex = 0;
        triggered = false;
        active = false;
        lastOkayPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool okayPressed = (Input.GetAxisRaw("AnyOkay") > 0.1f);

        if (!active)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(bounds.bounds.center, bounds.bounds.size, bounds.transform.rotation.z, LayerMask.GetMask("Player"));

            for (int i = 0; i < hits.Length; i++)
            {
                WaxController triggerController = hits[i].GetComponent<WaxController>();

                //TODO: Add Grounded condition
                if (triggerController != null && triggerController.grounded)
                {
                    if (!triggered || Input.GetAxisRaw("AnyNavV") < -0.5f)
                    {
                        triggered = true;
                        ActivateDialog();
                    }
                }
            }
        }
        else
        {
            if (okayPressed & !lastOkayPressed)
            {
                activeScreenIndex++;

                if (activeScreenIndex >= dialogScreens.Length)
                {
                    DeactivateDialog();
                }
                else
                {
                    UpdateDialog();
                }
            }
        }

        lastOkayPressed = okayPressed;
    }

    void ActivateDialog()
    {
        active = true;
        activeScreenIndex = 0;
        ProjectVars.EnterDialog();
        dialogDisplay.SetText(dialogScreens[activeScreenIndex]);
        dialogDisplay.transform.parent.gameObject.SetActive(true);
    }

    void UpdateDialog()
    {
        dialogDisplay.SetText(dialogScreens[activeScreenIndex]);
    }

    void DeactivateDialog()
    {
        active = false;
        ProjectVars.ExitDialog();
        dialogDisplay.transform.parent.gameObject.SetActive(false);
    }
}
