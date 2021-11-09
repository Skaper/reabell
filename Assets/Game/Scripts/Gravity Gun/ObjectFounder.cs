using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFounder : MonoBehaviour
{
    public float detectionDistance = 5f;
    public Transform[] targets;
    public AudioClip sound;
    public float minDelay = 0.3f;
    public float maxDelay = 2f;

    private AudioSource audioSource;
    private bool hasTarget = false;
    private float currentDelay;
    private float timer;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.clip = sound;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = foundTarget();
        if(distance <= detectionDistance)
        {
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
        }

        if (hasTarget)
        {
            currentDelay = Map(distance, 0f, detectionDistance, minDelay, maxDelay);
            timer += Time.deltaTime;
            if (!audioSource.isPlaying && timer >= currentDelay)
            {
                audioSource.Play();
                timer = 0;
            }
        }
    }

    private float foundTarget()
    {
        float minDistance = float.MaxValue;
        foreach(Transform target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < minDistance) minDistance = distance;
        }
        return minDistance;
    }

    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        if (x < in_min) return out_min;
        if (x > in_max) return out_max;
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}
