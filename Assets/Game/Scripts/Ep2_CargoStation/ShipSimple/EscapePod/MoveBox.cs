using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MoveBox : MonoBehaviour
{
    private Animator animator;
    private AudioSource audio;

    public UnityEvent onContact;
    void Start()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            print("Ship!");
            animator.SetTrigger("Drop");
            audio.Play();
            onContact.Invoke();
            enabled = false;
        }
    }
}
