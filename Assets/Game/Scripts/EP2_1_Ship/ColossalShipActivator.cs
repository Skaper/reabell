using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColossalShipActivator : MonoBehaviour
{
    public Animator smallScreenAnimator;
    public EnemyShipAI[] enemysAi;

    public GameObject beforeCrash;
    public GameObject afterCrash;

    public AudioSource explosionSound;
    public AudioSource aiSound;

    public AudioSource buttleSound;
    public AudioSource normalSound;

    public bool debug_activate = false;
    private bool isActivated = false;

    void Start()
    {
        afterCrash.SetActive(false);
        beforeCrash.SetActive(true);
        foreach (EnemyShipAI ai in enemysAi)
        {
            ai.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActivated && debug_activate)
        {
            ActivateShip();
        }
    }

    public void ActivateShip()
    {
        if(!isActivated)
        {
            normalSound.Stop();
            StartCoroutine(PlayAiSound());
            StartCoroutine(EnableShipsAI());
            smallScreenAnimator.Play("Computer");

            afterCrash.SetActive(true);
            beforeCrash.SetActive(false);
            isActivated = true;
        }
        
    }

    private IEnumerator PlayAiSound()
    {
        yield return new WaitForSeconds(1);
        if(!aiSound.isPlaying) aiSound.Play();
    }
    private IEnumerator EnableShipsAI()
    {
        yield return new WaitForSeconds(6f);
        explosionSound.Play();
        EZCameraShake.CameraShaker.Instance.ShakeOnce(4f, 4f, 1f, 1f);
        
        foreach (EnemyShipAI ai in enemysAi)
        {
            ai.enabled = true;
        }
        yield return new WaitForSeconds(2f);
        buttleSound.Play();
    }
}
