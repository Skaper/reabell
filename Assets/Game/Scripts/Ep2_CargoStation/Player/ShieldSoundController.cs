using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShieldSoundController : MonoBehaviour
{
    public AudioClip[] clipList;

    private AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;
    }

    public void PlayRandom()
    {
        if (!audio.isPlaying)
        {
            audio.PlayOneShot(clipList[Random.Range(0, clipList.Length)]);
        }
    }
    

}
