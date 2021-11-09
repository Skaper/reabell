using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FadeAudioSource
{

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float startVolume, float targetVolume)
    {
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public static IEnumerator StartFadePlay(AudioSource audioSource, float duration, float startVolume, float targetVolume)
    {
        audioSource.Play();
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public static IEnumerator StartFadePlay<T>(System.Action<bool> isOver, bool isPlay, AudioSource audioSource, float duration, float startVolume, float targetVolume)
    {
        isOver(false); 
        if(isPlay)audioSource.Play();
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        isOver(true);
        yield break;
    }
    public static IEnumerator StartFadeStop(AudioSource audioSource, float duration, float startVolume, float targetVolume)
    {
        
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            audioSource.volume = volume;
            yield return null;
        }
        audioSource.Stop();
        yield break;

    }

    public static IEnumerator StartFadeStop<T>(System.Action<bool> isOver, AudioSource audioSource, float duration, float startVolume, float targetVolume)
    {
        isOver(false);
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            Debug.Log("StartFadeStop: v: " + volume);
            audioSource.volume = volume;
            yield return null;
        }
        audioSource.Stop();
        isOver(true);
        yield break;

    }



}

