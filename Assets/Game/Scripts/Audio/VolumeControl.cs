
using UnityEngine;

public class VolumeControl: MonoBehaviour
{
    
    public SoundManager.SoundType soundType;

    private AudioSource audioSource;
    void Start()
    {
        SoundManager.onVolumeChanged += OnVolumeChange;
        audioSource = GetComponent<AudioSource>();
        SetupVolume();
    }

    void OnVolumeChange(SoundManager.SoundType type, float volume)
    {
        if (soundType != type) return;
        if (!audioSource) return;
        audioSource.volume = volume;
    }

    private void SetupVolume()
    {
        float volume = 0.5f;
        switch (soundType)
        {
            case SoundManager.SoundType.General:
                volume = PlayerPrefs.GetFloat("GeneralVolume", 1f);
                break;
            case SoundManager.SoundType.Effects:
                volume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
                break;
            case SoundManager.SoundType.Voice:
                volume = PlayerPrefs.GetFloat("VoiceVolume", 1f);
                break;
            case SoundManager.SoundType.Music:
                volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
                break;
        }

        audioSource.volume = volume;


    }
}
