using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIAudioPlayer : MonoBehaviour
{
    private AudioSource uiAudioSource;
    public AudioClip buttonClick;

    // Start is called before the first frame update
    void Start()
    {
        uiAudioSource = GetComponent<AudioSource>();
    }

    public void ButtonSound()
    {
        uiAudioSource.clip = buttonClick;
        uiAudioSource.Play();
    }
}
