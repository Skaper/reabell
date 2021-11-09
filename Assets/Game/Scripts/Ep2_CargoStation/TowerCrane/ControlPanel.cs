using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public Text displayText;
    public AudioClip acceptedSound;
    public AudioClip deniedSound;
    public AudioClip failedSound;

    private bool isPlayerIn = false;
    private int batteryCount = 0;
    private bool isShipCome = false;

    private bool isMakeSound = true;
    private float chargingTimer;
    private bool isStartCharging;
    private AudioSource audioSource;
    void Start()
    {
        GameManager.QuestManagerEp2.OnActionTowerCrane_BatteryAdd += OnActionTowerCrane_BatteryAdd;
        GameManager.QuestManagerEp2.OnActionShip_came += QuestManager_OnActionShip_came;
        audioSource = GetComponent<AudioSource>();
        GameManager.QuestManagerEp2.OnActionTowerCrane_Grab += StartCharging;
    }

    private void StartCharging()
    {
        isStartCharging = true;
        GameManager.QuestManagerEp2.OnActionTowerCrane_Grab -= StartCharging;
    }
    void Update()
    {
        

        if (batteryCount >= 3)
        {
            if (!isStartCharging) { 
                string text = "TARGET REQUEST";
                if (isShipCome) text = "TARGET FOUND";
                displayText.text = "ON" + batteryCount + "/3\n" + text;
                if (isShipCome) displayText.color = Color.green;
                else displayText.color = Color.red;
            }
            else
            {
                if(chargingTimer < QuestManagerEp2.ChargingTime ) chargingTimer += Time.deltaTime;
                float timeLeft = QuestManagerEp2.ChargingTime - chargingTimer;
                displayText.text = "Charging...\n" + Mathf.Round(timeLeft) + " s left";

            }

        }
        else
        {
            displayText.text = "OFF" + batteryCount + "/3";
        }

    }

    public void Use()
    {
        if (batteryCount == 3)
        {
            if (isShipCome)
            {
                audioSource.clip = acceptedSound;

                GameManager.QuestManagerEp2.OnActionControlPanel_access?.Invoke();
            }
            else
            {
                audioSource.clip = failedSound;
            }
        }
        else
        {
            audioSource.clip = deniedSound;
        }
        if (isMakeSound) audioSource.Play();
        if (isShipCome) isMakeSound = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        isPlayerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        isPlayerIn = false;
    }

    private void OnActionTowerCrane_BatteryAdd(int count)
    {
        batteryCount = count;
        if(count == 3) GameManager.QuestManagerEp2.OnActionTowerCrane_BatteryAdd -= OnActionTowerCrane_BatteryAdd;
        
    }

    private void QuestManager_OnActionShip_came()
    {
        isShipCome = true;
        
        GameManager.QuestManagerEp2.OnActionShip_came -= QuestManager_OnActionShip_came;
    }


}
