using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Ep2_1Intro : MonoBehaviour
{
    public float delayToPlayIntro = 5f;
    private AudioSource audio;

    public UnityEvent onPlay;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        StartCoroutine(PlayIntro());
    }
    IEnumerator  PlayIntro()
    {
        yield return new WaitForSeconds(delayToPlayIntro);
        audio.Play();
        onPlay?.Invoke();
    }
}
