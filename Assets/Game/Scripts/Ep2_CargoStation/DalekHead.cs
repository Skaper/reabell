using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class DalekHead : GrabbableEvents
{
    private bool isPlayed = false;

    private AudioSource audioSource;

    public override void OnGrab(Grabber grabber)
    {
        if (!isPlayed)
        {
            audioSource.Play();
            isPlayed = true;
        }
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
