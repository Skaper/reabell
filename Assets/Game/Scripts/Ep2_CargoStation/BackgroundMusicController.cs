using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioClip startMusic;
    public AudioSource startPlayer;
    public AudioClip fightMusic;
    public AudioSource fightPlayer;
    public AudioClip finishMusic;
    public AudioSource finishPlayer;


    void Start()
    {
        GameManager.QuestManagerEp2.OnAction_LevelStart += startLevel;
        GameManager.QuestManagerEp2.OnActionTowerCrane_Grab += startFight;
        GameManager.QuestManagerEp2.OnActionPlayerInsideShip += goAway;

        startPlayer.clip = startMusic;
        startPlayer.playOnAwake = true;
        startPlayer.Pause();

        fightPlayer.clip = fightMusic;
        fightPlayer.playOnAwake = true;
        fightPlayer.Pause();

        finishPlayer.clip = finishMusic;
        finishPlayer.playOnAwake = true;
        finishPlayer.Pause();
    }

    
    private void startLevel()
    {
        GameManager.QuestManagerEp2.OnAction_LevelStart -= startLevel;
        startPlayer.Play();
    }

    private void startFight()
    {
        GameManager.QuestManagerEp2.OnActionTowerCrane_Grab -= startFight;
        startPlayer.Stop();
        fightPlayer.Play();

    }

    private void goAway()
    {
        GameManager.QuestManagerEp2.OnActionPlayerInsideShip -= goAway;
        fightPlayer.Stop();
        finishPlayer.Play();
    }
}
