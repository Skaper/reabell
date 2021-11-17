using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class OxygenController : MonoBehaviour
{
    public CircleProgressBar progressBar;
    public GameObject audioEffector;

    public AudioClip hardBreathing;

    private AudioSource audio;


    private PlayerStats stats;
    private bool isStartUsingOxygen = false;
    private float oxygenTimer;
    private float healthTimer;
    private VignetteAndChromaticAberration vignette;
    private float vignetteIntensity = 0f;
    private bool hasSendWarningMassage = false;
    public Invector.PlayerHealthController playerHealth;
    
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        progressBar.maxValue = (int)stats.OxygenMax;
        progressBar.value = (int)stats.OxygenLevel;
        //TODO: УБРАТЬ ЗАВИСИМОСТЬ!
        GameManager.QuestManagerEp2.OnActionAirCompressor_off += OnActionAirCompressor_off;
        vignette = GameManager.PlayerCamera.GetComponent<VignetteAndChromaticAberration>();
        //playerHealth = GetComponent<Invector.PlayerHealthController>();

        GameObject soundEffector = Instantiate(audioEffector, transform.position, transform.rotation);
        soundEffector.GetComponent<AudioEffect>().destroyAfterPlaying = false;
        soundEffector.transform.SetParent(transform);
        audio = soundEffector.GetComponent<AudioSource>();
        audio.clip = hardBreathing;
        audio.loop = true;
    }
    private void OnActionAirCompressor_off()
    {
        isStartUsingOxygen = true;
        GameManager.QuestManagerEp2.OnActionAirCompressor_off -= OnActionAirCompressor_off;
    }


    void Update()
    {
        if (isStartUsingOxygen)
        {
            if (stats.OxygenLevel > 0) audio.loop = false;
            oxygenTimer += Time.deltaTime;
            if(oxygenTimer >= 0.1f)
            {
                stats.TickOxygen();
                oxygenTimer = 0f;
            }
            if (stats.OxygenLevel <= 20f)
            {
                if (!hasSendWarningMassage)
                {
                    GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e10");
                    hasSendWarningMassage = true;
                }
                vignetteIntensity = Map(20f - stats.OxygenLevel, 0, 20f, 0f, 10f); 
                vignette.intensity = vignetteIntensity;
                vignette.blur = vignetteIntensity;
                if(stats.OxygenLevel <= 0) {
                    if (!audio.isPlaying) audio.Play();
                    audio.loop = true;
                    if (healthTimer >= 0.1f)
                    {
                        playerHealth.TakeDamage(stats.HealthTick);
                        healthTimer = 0f;
                    }
                    healthTimer += Time.deltaTime;
                }
            }
            else
            {
                hasSendWarningMassage = false;
                if (vignetteIntensity > 0.001f) ReturnVision();
            }
        }
        
    }

    private void FixedUpdate()
    {
        progressBar.value = (int)stats.OxygenLevel;
    }

    private void ReturnVision()
    {
        vignette.intensity -= vignette.intensity * Time.deltaTime;
        vignette.blur -= vignette.blur * Time.deltaTime;
        vignetteIntensity = vignette.intensity;
    }

    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        if (x < in_min) return out_min;
        if (x > in_max) return out_max;
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
