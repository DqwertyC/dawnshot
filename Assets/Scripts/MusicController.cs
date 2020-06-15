using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (_audioSource != null)
        {
            if (_audioSource.isPlaying) return;
            _audioSource.Play();
        }
        
    }

    public void StopMusic()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }
}
