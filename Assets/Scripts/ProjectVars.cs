using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectVars
{

    public static bool GAME_PAUSED = false;
    public static bool IN_DIALOG = false;
    public static bool IN_DEATH_MENU = false;

    public static void TogglePause()
    {
        if (!IN_DEATH_MENU) GAME_PAUSED = !GAME_PAUSED;
        UpdateTimeScale();
    }

    public static void Pause()
    {
        GAME_PAUSED = true;
        UpdateTimeScale();
    }

    public static void UnPause()
    {
        GAME_PAUSED = false;
        UpdateTimeScale();
    }

    public static void EnterDialog()
    {
        IN_DIALOG = true;
        UpdateTimeScale();
    }

    public static void ExitDialog()
    {
        IN_DIALOG = false;
        UpdateTimeScale();
    }

    public static void EnterDeath()
    {
        IN_DEATH_MENU = true;
        UpdateTimeScale();
    }

    public static void ExitDeath()
    {
        IN_DEATH_MENU = false;
        UpdateTimeScale();
    }

    static void UpdateTimeScale()
    {
        if (GAME_PAUSED || IN_DIALOG || IN_DEATH_MENU)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public static Color SetAlpha(this Color c, float alpha)
    {

        return new Color(c.r, c.g, c.b, alpha);
    }
}
