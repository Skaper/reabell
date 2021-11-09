using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioQueue : MonoBehaviour
{

    [Header("Queue list:")]
    public AudioClip[] clips;
    [Tooltip("The clip index from which the queue will begin:")]
    public int startClipNumber = 0;
    [Tooltip("Loop only one clip by index:")]
    public int loopingClipNumber = -1;
    [Tooltip("Loop all the queue:")]
    public bool loopAllQueue = false;

    private AudioSource audioSource;
    private int priviusClipIndex = -1;

    private int currentClipIndex;

    private bool isPlaying = false;

    

    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        currentClipIndex = startClipNumber;
    }

    

    void Update()
    {
        if (currentClipIndex > clips.Length - 1 && !audioSource.isPlaying)
        {
            if (loopAllQueue) currentClipIndex = startClipNumber;
            else isPlaying = false;
        }
        if (!audioSource.isPlaying && isPlaying && priviusClipIndex != currentClipIndex)
        {
            audioSource.clip = clips[currentClipIndex];
            audioSource.Play();
            if(currentClipIndex != loopingClipNumber) { 
                priviusClipIndex = currentClipIndex;
                currentClipIndex++;
            }
        }
        
    }

    public void Play()
    {
        isPlaying = true;
        currentClipIndex = startClipNumber;
    }

    public void Play(int index)
    {
        isPlaying = true;
        if (index >= clips.Length) currentClipIndex = clips.Length - 1;
        else if (index < 0) index = 0;
        else currentClipIndex = index;
    }

    public void Stop()
    {
        isPlaying = false;
        audioSource.Stop();
    }

    public int Length()
    {
        return clips.Length;
    }

    public int PlaingClipNumber()
    {
        return currentClipIndex;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void setLoopClip(int clipNumber, bool loop)
    {
        if (loop) loopingClipNumber = clipNumber;
        else loopingClipNumber = -1;
    }

    

}

