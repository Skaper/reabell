using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeTrigger : MonoBehaviour
{
    public Animator EscapePodAnimator;
    private AudioSource openSound;
    private bool isOpen;
    private void Start()
    {
        openSound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen && other.CompareTag("Player"))
        {
            EscapePodAnimator.Play("EscapePodOpen");
            openSound.Play();
            isOpen = true;
        }
    }
}
