using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class StandardMenu : MonoBehaviour
{
    public void PlayTutorial()
    {
        ProjectVars.UnPause();
        ProjectVars.ExitDeath();
        SceneManager.LoadScene("Tutorial");
    }

    public void PlaySandbox()
    {
        ProjectVars.UnPause();
        ProjectVars.ExitDeath();
        SceneManager.LoadScene("Sandbox");
    }

    public void PlayChallenge()
    {
        ProjectVars.UnPause();
        ProjectVars.ExitDeath();
        SceneManager.LoadScene("Challenge");
    }

    public void ResetGame()
    {
        ProjectVars.UnPause();
        ProjectVars.ExitDeath();
        Invoke("ResetScene", 0.25f);
    }

    private void ResetScene()
    {
        Debug.Log("Resetting");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void QuitToMenu()
    {
        ProjectVars.UnPause();
        ProjectVars.ExitDeath();
        SceneManager.LoadScene("StartMenu");
    }
}
