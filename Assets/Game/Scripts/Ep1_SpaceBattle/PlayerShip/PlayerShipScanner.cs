using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipScanner : MonoBehaviour
{
    public Action<string> onDataIncome;
    public Action<bool> onScannerChangedState;

    public Transform rayPoint;
    public ParticleSystem particleSystem;
    public float distanceToScan = 26f;

    private AudioSource audioSource;
    private Animator rayAnimator;

    private bool wasScannerDataShown;
    private float scannerDataShowTimer;
    private float scannerDataShowDelay = 3f;

    private bool scannerButtonPressed;

    public bool isScannerWorking { get; private set; }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rayAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (scannerButtonPressed)
        {
            //isScannerWorking = !isScannerWorking;
            if (isScannerWorking)
            {
                onScannerChangedState?.Invoke(true);
                onDataIncome?.Invoke("Scanning...");
                if (!particleSystem.isPlaying) particleSystem.Play();
                audioSource.Play();
                rayAnimator.SetTrigger("ScennerOn");
            }
            else
            {
                onScannerChangedState?.Invoke(false);
                if (particleSystem.isPlaying) particleSystem.Stop();
                audioSource.Stop();
                rayAnimator.SetTrigger("ScannerOff");
            }
            scannerButtonPressed = false;
        }

        if (isScannerWorking)
        {
            RaycastHit hit;
            Physics.Raycast(
                rayPoint.position,
                rayPoint.TransformDirection(Vector3.forward),
                out hit,
                distanceToScan,
                1 << 0,
                QueryTriggerInteraction.Ignore
                );
            if (hit.collider)
            {
                ScannerData scannerData = hit.collider.gameObject.GetComponent<ScannerData>();
                if(scannerData != null)
                {
                    onDataIncome?.Invoke(scannerData.Data);
                    wasScannerDataShown = true;
                    scannerDataShowTimer = 0f;
                }
            }
        }

        if (wasScannerDataShown)
        {
            scannerDataShowTimer += Time.deltaTime;
            if (scannerDataShowTimer >= scannerDataShowDelay)
            {
                scannerDataShowTimer = 0f;
                wasScannerDataShown = false;
                onDataIncome?.Invoke("Scanning...");
            }
        }
    }

    public void DoScanning()
    {
        scannerButtonPressed = true;
        isScannerWorking = true;
    }

    public void StopScanning()
    {
        scannerButtonPressed = true;
        isScannerWorking = false;
    }
}
