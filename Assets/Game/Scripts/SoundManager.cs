using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class SoundManager

{
    public enum SoundType
    {
        General,
        Effects,
        Voice,
        Music
    }
    public static Action<SoundType, float> onVolumeChanged;
}
