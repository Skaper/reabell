using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPanelExplosion : MonoBehaviour
{
    public AudioSource explosionSound;
    // Start is called before the first frame update
    public Animator animator;
    public ParticleSystem explosion1;
    public ParticleSystem explosion2;

    private bool isExployded = false;
    void Start()
    {
        
    

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isExployded)
        {
            animator.Play("PanelExplosion");
            explosion1.Play();
            explosion2.Play();
            isExployded = true;
            EZCameraShake.CameraShaker.Instance.ShakeOnce(4f, 4f, 1f, 1f);
            explosionSound.Play();
        }
    }
}
