using System;
using System.Collections.Generic;
using UnityEngine;

public class SocketTrigger : MonoBehaviour
{
    public GameObject socket1;
    public GameObject socket2;
    public GameObject socket3;

    public AudioClip batteryAttachSound;

    public float attractSpeed = 2f;
    public float rotateSpeed = 5f;

    private int batteryCount = 0;

    private GameObject currentBattery = null;
    private GameObject currentSocket = null;
    private Transform currentHologramTransform = null;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.tag != "Block") return;
        string goName = go.name;
        if (!goName.Contains("Battery")) return;
        currentBattery = go;
        switch (goName)
        {
            case "Battery1":
                currentSocket = socket1;
                
                batteryCount++;
                break;
            case "Battery2":
                currentSocket = socket2;
                batteryCount++;
                break;
            case "Battery3":
                currentSocket = socket3;
                batteryCount++;
                break;
        }
        if (currentSocket) {
            currentHologramTransform = currentSocket.transform.GetChild(0).gameObject.transform;
            DisambleBatteryPhysics(currentSocket, currentBattery);
        }

    }

    private void FixedUpdate()
    {
        if(currentSocket != null)
        {
            bool isRotateComplite = false;
            bool isMovingComplit = false;
            if(CheckRotation() > 0.05f)
            {
                RotateBatteryToSocket();
            }
            else
            {
                isRotateComplite = true;
            }
            if (CheckDist() > 0.05f)
            {
                MoveBatteryToSocket();
            }
            else
            {
                isMovingComplit = true;
            }
            if(isMovingComplit && isRotateComplite)
            {
                currentSocket = null;
                currentBattery = null;
                if(audioSource && batteryAttachSound)
                {
                    audioSource.clip = batteryAttachSound;
                    audioSource.Play();
                }
                
                GameManager.QuestManagerEp2.OnActionTowerCrane_BatteryAdd?.Invoke(batteryCount);
                switch (batteryCount)
                {
                    case 1:
                        GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e_b1");
                        break;
                    case 2:
                        GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e_b2");
                        break;
                    case 3:
                        GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e_b3");
                        break;
                }
            }
        }
    }

    private void DisambleBatteryPhysics(GameObject socket, GameObject battery)
    {
        GravityGunIteractiveObject ggio = battery.GetComponent<GravityGunIteractiveObject>();
        ggio.DisablePhysics();
        ggio.inGravityGun = false;
        battery.transform.SetParent(socket.transform);
        socket.transform.GetChild(0).gameObject.SetActive(false);
    }

    public float CheckDist()
    {
        float dist = Vector3.Distance(currentBattery.transform.position, currentHologramTransform.position);
        return dist;
    }
    private float CheckRotation()
    {
        return Quaternion.Angle(currentBattery.transform.rotation, currentHologramTransform.rotation);
    }
    private void MoveBatteryToSocket()
    {
        currentBattery.transform.position = Vector3.Lerp(
            currentBattery.transform.position, 
            currentSocket.transform.position, 
            attractSpeed * Time.deltaTime
            );
        
        
    }

    private void RotateBatteryToSocket()
    {
        currentBattery.transform.rotation = Quaternion.RotateTowards(
            currentBattery.transform.rotation,
            currentHologramTransform.rotation,
            Time.deltaTime * rotateSpeed
            );
    }

}
