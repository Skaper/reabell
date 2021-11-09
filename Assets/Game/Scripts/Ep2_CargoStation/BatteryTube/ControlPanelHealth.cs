using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Invector
{
    
    public class ControlPanelHealth : vHealthController
    {
        [vEditorToolbar("Settings", order = 200)]
        public Slider slider;
        public ParticleSystem fx;
        public AudioClip destroyClip;
        public GameObject tube;
        public GameObject battery;
        public Transform batteryTarget;
        public Transform batteryStartPosition;
        public float attractSpeed = 2f;

        private bool isDestroy = false;
        private AudioSource audioSource;
        private GravityGunIteractiveObject baterryGGIO;
        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            slider.maxValue = MaxHealth;
            if(fx.isPlaying) fx.Stop();
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.clip = destroyClip;

            baterryGGIO = battery.GetComponent<GravityGunIteractiveObject>();
            baterryGGIO.DisablePhysics();
            battery.transform.position = batteryStartPosition.position;
        }

        // Update is called once per frame
        void Update()
        {
            
            slider.value = currentHealth;
            if(currentHealth <= 0 && !isDestroy)
            {
                if (!fx.isPlaying) fx.Play();
                audioSource.Play();
                isDestroy = true;
            }

            if (isDestroy)
            {
                if (Vector3.Distance(battery.transform.position, batteryTarget.position) > 0.001f)
                {
                    MoveBattery();
                }
                else
                {
                    baterryGGIO.EnablePhysics();
                    enabled = false;
                }
            }
        }
        private void MoveBattery()
        {
            battery.transform.position = Vector3.Lerp(
                battery.transform.position,
                batteryTarget.position,
                attractSpeed * Time.deltaTime
                );


        }
    }

    
}

