using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaxSounds : MonoBehaviour
{

    private Random clipSelector;
    private AudioSource audioPlayer;

    public AudioClip[] stepClips;
    public AudioClip[] jumpClips;
    public AudioClip[] landClips;
    public AudioClip[] hurtClips;
    public AudioClip[] grabClips;
    public AudioClip[] vialClips;

    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        clipSelector = new Random();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Step()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.clip = stepClips[Random.Range(0,stepClips.Length)];
            audioPlayer.Play();
        }
    }

    public void Jump()
    {
        if (audioPlayer.isPlaying) audioPlayer.Stop();
        audioPlayer.clip = jumpClips[Random.Range(0, jumpClips.Length)];
        audioPlayer.Play();
    }

    public void ImpactNormal()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.clip = landClips[Random.Range(0, landClips.Length)];
            audioPlayer.Play();
        }
    }

    public void ImpactHurt()
    {
        if (audioPlayer.isPlaying) audioPlayer.Stop();
        audioPlayer.clip = hurtClips[Random.Range(0, hurtClips.Length)];
        audioPlayer.Play();
    }

    public void ItemGet()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.clip = grabClips[Random.Range(0, grabClips.Length)];
            audioPlayer.Play();
        }
    }

    public void DrinkVial()
    {
        if (audioPlayer.isPlaying) audioPlayer.Stop();
        audioPlayer.clip = vialClips[Random.Range(0, vialClips.Length)];
        audioPlayer.Play();
    }
}
