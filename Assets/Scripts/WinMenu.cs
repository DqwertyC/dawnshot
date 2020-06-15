using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinMenu : StandardMenu
{
    public Image menuBackground;
    public TextMeshProUGUI menuTitle;
    public GameObject menuObjects;
    public WaxController wax;

    float timer;

    float timeToStart = 0.5f;
    float timeToMessage = 2.0f;
    float timeToMenu = 3.0f;

    private enum State
    {
        FINISHED,
        WAITING,
        FADING_TITLE,
        FADING_MENU,
        DONE
    }

    State currentState;
    State nextState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.FINISHED;
    }

    // Update is called once per frame
    void Update()
    {
        nextState = currentState;

        if (State.FINISHED == currentState)
        {
            if (wax.isDone)
            {
                nextState = State.WAITING;
                timer = 0;
            }
        }
        else if (State.WAITING == currentState)
        {
            if (timer >= timeToStart)
            {
                timer = 0;

                menuBackground.gameObject.SetActive(true);

                menuBackground.color = menuBackground.color.SetAlpha(0.0f);
                menuTitle.color = menuTitle.color.SetAlpha(0.0f);

                menuBackground.enabled = true;

                nextState = State.FADING_TITLE;
            }
        }
        else if (State.FADING_TITLE == currentState)
        {
            if (timer < timeToMessage)
            {
                float alpha = timer / timeToMessage;

                menuBackground.color = menuBackground.color.SetAlpha(0.5f * alpha);
                menuTitle.color = menuTitle.color.SetAlpha(alpha);
            }
            else
            {
                timer = 0;
                menuObjects.SetActive(true);
                ProjectVars.EnterDeath();
                nextState = State.DONE;
            }
        }
        else if (State.DONE == currentState)
        {
            if (!ProjectVars.IN_DEATH_MENU)
            {
                menuObjects.SetActive(false);
                menuBackground.gameObject.SetActive(false);
                nextState = State.FINISHED;
            }
        }

        currentState = nextState;
        timer += Time.deltaTime;
    }
}
