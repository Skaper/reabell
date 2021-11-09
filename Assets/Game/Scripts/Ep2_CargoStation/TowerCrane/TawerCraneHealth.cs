using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Invector
{
    [RequireComponent (typeof(AudioSource))]
    public class TawerCraneHealth : vHealthController
    {
        [vEditorToolbar("Settings", order = 200)]
        public Slider slider;
        public ParticleSystem fx;
        public AudioClip destroyClip;

        private bool isDestroy = false;
        private AudioSource audioSource;
        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            slider.maxValue = MaxHealth;
            if (fx.isPlaying) fx.Stop();
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.clip = destroyClip;

        }

        // Update is called once per frame
        void Update()
        {

            slider.value = currentHealth;
            if (currentHealth <= 0 && !isDestroy)
            {
                if (!fx.isPlaying) fx.Play();
                audioSource.Play();
                isDestroy = true;
            }

            if (isDestroy)
            {
                
            }
        }
    }


}