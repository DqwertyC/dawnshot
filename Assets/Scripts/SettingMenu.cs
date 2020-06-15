using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider effectSlider;

    public AudioMixer mainMixer;


    public void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicLevel", 0);
        effectSlider.value = PlayerPrefs.GetFloat("EffectLevel", 0);

        UpdateVolume();
    }

    public void SetGamepad(bool controllerEnabled)
    {
        PlayerPrefs.SetInt("InputMode", controllerEnabled ? 1 : 0);
    }

    public void UpdateVolume()
    {
        PlayerPrefs.SetFloat("EffectLevel", effectSlider.value);
        PlayerPrefs.SetFloat("MusicLevel", musicSlider.value);

        mainMixer.SetFloat("MusicVolume", (musicSlider.value > -7 ? 4 : 10) * musicSlider.value);
        mainMixer.SetFloat("EffectVolume", (effectSlider.value > -7 ? 4 : 10) * effectSlider.value);
    }
}
