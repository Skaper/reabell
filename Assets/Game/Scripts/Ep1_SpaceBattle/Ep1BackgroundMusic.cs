using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ep1BackgroundMusic : MonoBehaviour
{
    public AudioSource mainMusicAudio;
    public AudioSource battleMusicAudio;

    private float mainMusicDefaultVolume;
    private float battleMusicDefaultVolume;
    void Start()
    {
        GameManager.QuestManagerEp1.onActionPlayerShipActive += actionPlayerShipActive;
        GameManager.QuestManagerEp1.onActionBeaconDestroyed += actionBeaconDestroyed;
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed += actionEnemiesDestroyed;

        mainMusicDefaultVolume = mainMusicAudio.volume;
        battleMusicDefaultVolume = battleMusicAudio.volume;
    }

    private void actionPlayerShipActive()
    {
        StartCoroutine(FadeAudioSource.StartFadePlay(mainMusicAudio, 3f, 0f, mainMusicDefaultVolume));
        //mainMusicAudio.Play();
        GameManager.QuestManagerEp1.onActionPlayerShipActive -= actionPlayerShipActive;
    }

    private void actionBeaconDestroyed()
    {
        StartCoroutine(FadeAudioSource.StartFadeStop(mainMusicAudio, 3f, mainMusicDefaultVolume, 0f));
        StartCoroutine(FadeAudioSource.StartFadePlay(battleMusicAudio, 3f, 0f, battleMusicDefaultVolume));
        //mainMusicAudio.Pause();
        //battleMusicAudio.Play();
        GameManager.QuestManagerEp1.onActionBeaconDestroyed -= actionBeaconDestroyed;
    }

    private void actionEnemiesDestroyed()
    {
        StartCoroutine(FadeAudioSource.StartFadeStop(battleMusicAudio, 3f, battleMusicDefaultVolume, 0f));
        StartCoroutine(FadeAudioSource.StartFadePlay(mainMusicAudio, 3f, 0f, mainMusicDefaultVolume));
        //battleMusicAudio.Stop();
        //mainMusicAudio.UnPause();
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed -= actionEnemiesDestroyed;
    }
}
