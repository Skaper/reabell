using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffect : MonoBehaviour
{


    private AudioSource audioSource;
    public bool destroyAfterPlaying = false;

    private bool hasPlayed = false;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        
        bool isPlaying = audioSource.isPlaying;
        if (isPlaying && !hasPlayed)
        {
            hasPlayed = true;
        }

        if(!isPlaying && hasPlayed)
        {
            if (destroyAfterPlaying)
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }
}
