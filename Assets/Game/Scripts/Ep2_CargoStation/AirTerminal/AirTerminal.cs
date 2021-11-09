using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;

public class AirTerminal : MonoBehaviour
{
    public Text displayText;
    public Lever lever;
    public GameObject battery;
    public GameObject shield;

    public GameObject compressor1;
    public GameObject compressor2;

    public AudioClip deniedSound;

    public GameObject scannerTarget;
    public float attractSpeed = 2f;
    public float rotateSpeed = 100f;

    private bool isPlayerIn = false;

    private bool isAirDisabled = false;
    private bool isButtonPresed = false;
    private AudioSource audioSource;
    private AudioQueue audioQueue;

    private GameObject scanner = null;
    private bool isScannerIn = false;

    private bool startToDisamble = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioQueue = GetComponent<AudioQueue>();
        battery.tag = "Untagged";
        lever.enabled = false;
    }

    void Update()
    {
        if (isScannerIn)
        {
            lever.enabled = true;
        }
        if (isPlayerIn)
        {
            
            if (!isButtonPresed && isScannerIn && GameManager.QuestManagerEp2.towerCrane_batteryCount <= 1)
            {
                GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e3_1");
            }
            if (!isButtonPresed && isScannerIn && GameManager.QuestManagerEp2.towerCrane_batteryCount == 2)
            {
                GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e3_2");
            }
        }
        
        if (!startToDisamble && audioQueue.PlaingClipNumber() == 2)
        {
            //isButtonPresed = false;
            startToDisamble = true;
            DisableAir();
            GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e3_3");
        }

        if (scanner)
        {
            if(Vector3.Distance(scanner.transform.position, scannerTarget.transform.position) > 0.001f) MoveScannerToSocket();
            RotateScanneToSocket();
            isScannerIn = true;
        }


    }

    public void DisableAirWithLever()
    {
        if (!isAirDisabled && isScannerIn && !isButtonPresed)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            audioQueue.Play();
            isButtonPresed = true;
        }
    }
    private void DisableAir()
    {
        isAirDisabled = true;
        shield.active = false;
        battery.GetComponent<Rigidbody>().isKinematic = false;
        battery.tag = "Block";
        displayText.text = "AIR COMPRESSOR IS OFF";
        displayText.color = Color.red;
        compressor1.GetComponent<AudioSource>().Stop();
        compressor2.GetComponent<AudioSource>().Stop();
        GameManager.QuestManagerEp2.OnActionAirCompressor_off?.Invoke();
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

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.tag != "Block") return;
        string goName = go.name;
        if (!goName.Contains("idScanner")) return;

        scanner = go;
        scanner.GetComponent<Grabbable>().DropItem(true, true);
        scanner.GetComponent<GravityGunIteractiveObject>().DisablePhysics();
        scanner.GetComponent<Grabbable>().enabled = false;;
        scannerTarget.active = false;
    }

    

    private void MoveScannerToSocket()
    {
        scanner.transform.position = Vector3.Lerp(
            scanner.transform.position,
            scannerTarget.transform.position,
            attractSpeed * Time.deltaTime
            );


    }

    private void RotateScanneToSocket()
    {
        scanner.transform.rotation = Quaternion.RotateTowards(
            scanner.transform.rotation,
            scannerTarget.transform.rotation,
            Time.deltaTime * rotateSpeed
            );
    }

}
