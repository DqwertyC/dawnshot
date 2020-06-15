using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChallengeMenu : StandardMenu
{
    public Image menuBackground;
    public TextMeshProUGUI menuTitle;
    public TextMeshProUGUI scoreDisplay;
    public GameObject menuObjects;
    public WaxController wax;

    float timer;

    float timeToStart = 0.5f;
    float timeToMessage = 2.0f;
    float timeToMenu = 3.0f;

    private enum State
    {
        ALIVE,
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
        currentState = State.ALIVE;
    }

    // Update is called once per frame
    void Update()
    {
        nextState = currentState;

        if (State.ALIVE == currentState)
        {
            if (wax.isDead)
            {
                nextState = State.WAITING;
                timer = 0;
                int thisScore = (int)Mathf.Abs(wax.transform.position.x) - 3;
                int maxScore = PlayerPrefs.GetInt("ChallengeScore", 0);

                if (thisScore > maxScore)
                {
                    PlayerPrefs.SetInt("ChallengeScore", thisScore);
                    scoreDisplay.SetText("YOU TRAVELLED " + thisScore + " METERS\n" +
                                         "THAT'S A NEW RECORD!");
                }
                else if (thisScore == maxScore)
                {
                    scoreDisplay.SetText("YOU TRAVELLED " + thisScore + " METERS\n" +
                                         "YOU TIED WITH THE RECORD!");
                }
                else
                {
                    scoreDisplay.SetText("YOU TRAVELLED " + thisScore + " METERS\n" +
                                         "THE RECORD IS " + maxScore + " METERS");
                }
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
                scoreDisplay.gameObject.SetActive(true);

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
                nextState = State.ALIVE;
            }
        }

        currentState = nextState;
        timer += Time.deltaTime;
    }

    public void Continue()
    {
        ProjectVars.ExitDeath();
        ResetGame();
    }
}
