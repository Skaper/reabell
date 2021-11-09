
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class TouchButton : MonoBehaviour
{
    public Transform button;

    public Transform realized;
    public Transform pressed;

    public bool holdAfterPress = false;

    public string collisionContainsTag = "Player";

    public UnityEvent onPressed;
    public UnityEvent onRealized;



    private bool isPressed = false;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        if (tag.Contains(collisionContainsTag))
        {
            Button();
        }
        if (!holdAfterPress)
        {
            StartCoroutine(ReturnButton());
        }
    }

    private IEnumerator ReturnButton()
    {
        yield return new WaitForSeconds(1);
        Button();
    }

    private void Button()
    {
        if (isPressed)
        {
            //StartCoroutine(MoveButton(realized.position));
            button.position = realized.position;
            onRealized?.Invoke();
            isPressed = false;
            audioSource.Play();
        }
        else
        {
            //StartCoroutine(MoveButton(pressed.position));
            button.position = pressed.position;
            onPressed?.Invoke();
            isPressed = true;
            audioSource.Play();
        }
    }
}
