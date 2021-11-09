using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VolumeControl))]
public class SoundTrigger : MonoBehaviour
{
    public string reactionTrigger = "Player";
    public bool playOnce = true;

    private bool isPlayed = false;
    private AudioSource audioSource;

    public UnityEvent onPlayerEnter;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playOnce)
        {
            if (!isPlayed)
            {
                audioSource.Play();
                isPlayed = true;
                onPlayerEnter?.Invoke();
            }
        }
        else
        {
            if(!audioSource.isPlaying) audioSource.Play();
        }
    }
}
