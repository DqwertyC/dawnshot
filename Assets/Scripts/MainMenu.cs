using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MainMenu : StandardMenu
{
    public Button[] TutorialRequired;

    void Start()
    {
        if (0 == PlayerPrefs.GetInt("TutorialComplete", 0))
        {
            foreach (Button b in TutorialRequired)
            {
                b.interactable = false;
                TextMeshProUGUI t = b.GetComponentInChildren<TextMeshProUGUI>();
                t.color = t.color.SetAlpha(0.5f);
            }
        }
    }
}
