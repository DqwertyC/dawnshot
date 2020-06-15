using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CustomToggle : MonoBehaviour
{
    public TextMeshProUGUI template;
    public string toggleOn;
    public string toggleOff;

    private Toggle toggle;
    bool lastToggle;


    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = PlayerPrefs.GetInt("InputMode",0) == 1;
    }

    // Update is called once per frame
    void Update()
    {
        template.text = toggle.isOn ? toggleOn : toggleOff;
    }
}
