using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum SoundType
    {
        General,
        Effects,
        Voice,
        Music
    }

    public Action<SoundType, float> OnActionVolumeChange; 

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
