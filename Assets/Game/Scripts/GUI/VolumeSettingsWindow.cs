using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettingsWindow : MonoBehaviour
{
    public SliderVrTouch general;
    public SliderVrTouch effects;
    public SliderVrTouch voice;
    public SliderVrTouch music;
    void Start()
    {
        //global.SetValue(PlayerPrefs.GetFloat("GlobalVolume", 1f));
        general.SetValue(PlayerPrefs.GetFloat("GeneralVolume", 1f));
        effects.SetValue(PlayerPrefs.GetFloat("EffectsVolume", 1f));
        voice.SetValue(PlayerPrefs.GetFloat("VoiceVolume", 1f));
        music.SetValue(PlayerPrefs.GetFloat("MusicVolume", 1f));
    
    }

    public void Save()
    {
        float gV = general.value;
        PlayerPrefs.SetFloat("GeneralVolume", gV);
        SoundManager.onVolumeChanged?.Invoke(SoundManager.SoundType.General, gV);

        float eV = effects.value;
        PlayerPrefs.SetFloat("EffectsVolume", eV);
        SoundManager.onVolumeChanged?.Invoke(SoundManager.SoundType.Effects, eV);

        float vV = voice.value;
        PlayerPrefs.SetFloat("VoiceVolume", vV);
        SoundManager.onVolumeChanged?.Invoke(SoundManager.SoundType.Voice, vV);

        float mV = music.value;
        PlayerPrefs.SetFloat("MusicVolume", mV);
        SoundManager.onVolumeChanged?.Invoke(SoundManager.SoundType.Music, mV);

        PlayerPrefs.Save();
    }
}
