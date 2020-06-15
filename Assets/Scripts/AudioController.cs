using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public bool playMusic;
    public bool playWind;

    // Start is called before the first frame update
    void Start()
    {
        MusicController music = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicController>();
        MusicController wind = GameObject.FindGameObjectWithTag("Wind").GetComponent<MusicController>();

        if (music != null)
        {
            if (playMusic)
                music.PlayMusic();
            else
                music.StopMusic();
        }


        if (wind != null)
        {
            if (playWind)
                wind.PlayMusic();
            else
                wind.StopMusic();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
