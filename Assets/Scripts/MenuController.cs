using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuObject;

    DiscreteHeldInput heldMenu;
    DiscreteHeldInput heldNavV;
    DiscreteHeldInput heldNavH;

    // Start is called before the first frame update
    void Start()
    {
        heldMenu = new DiscreteHeldInput(float.MaxValue, float.MaxValue);
        heldNavV = new DiscreteHeldInput(0.5f, 0.2f);
        heldNavH = new DiscreteHeldInput(0.5f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        float menu = Input.GetAxisRaw("AnyMenu");
        float navV = Input.GetAxisRaw("AnyNavV");
        float navH = Input.GetAxisRaw("AnyNavH");

        if (heldMenu.Update(menu) == 1)
        {
            ProjectVars.TogglePause();

            if (ProjectVars.GAME_PAUSED)
            {
                menuObject.SetActive(true);
            }
            else
            {
                menuObject.SetActive(false);
            }
        }
    }

    public void TogglePauseMenu()
    {
        ProjectVars.TogglePause();

        if (ProjectVars.GAME_PAUSED)
        {
            menuObject.SetActive(true);
        }
        else
        {
            menuObject.SetActive(false);
        }
    }
}
