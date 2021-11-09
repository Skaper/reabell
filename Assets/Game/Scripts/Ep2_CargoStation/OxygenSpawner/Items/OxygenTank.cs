using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;
public class OxygenTank : GrabbableEvents
{
    public Lever wheelLever;
    public ParticleSystem bubblesFX;
    public UnityEngine.UI.Slider capacitySlider;
    public AudioSource airSound;

    public float oxygenLeakSpeed = 1f;

    private Grabber currentGrabber;
    private PlayerStats playerStats;

    private float capacity;
    private float m_capacity;

    void Start()
    {
        if (bubblesFX.isPlaying) bubblesFX.Stop();
        playerStats = GameManager.Player.GetComponent<PlayerStats>();
        capacity = playerStats.OxygenMax;
        m_capacity = capacity;
    }

    void Update()
    {
        
        float leaverLevel = wheelLever.LeverPercentage;
        if(capacity > 0 && (leaverLevel < 35 || leaverLevel > 65))
        {
            bubblesFX.Play();
            if(!airSound.isPlaying) airSound.Play();
            float oxygenOut = playerStats.OxygenMax * oxygenLeakSpeed * Time.deltaTime;
            capacity -= oxygenOut;
            if (capacity < 0) capacity = 0;
            playerStats.AddOxygen(oxygenOut);
        }
        else
        {
            airSound.Stop();
            bubblesFX.Stop();
        }
        
        if(m_capacity != capacity)
        {
            capacitySlider.value = capacity / playerStats.OxygenMax * 100f;
            m_capacity = capacity;
        }
    }

    public override void OnRelease()
    {
        currentGrabber = null;
    }

    public override void OnGrab(Grabber grabber)
    {
        currentGrabber = grabber;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    GameManager.Player.GetComponent<PlayerStats>().AddOxygen();
        //    GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e4");
        //    if (destroyAfterEnter) Destroy(gameObject);
        //}
    }
}
