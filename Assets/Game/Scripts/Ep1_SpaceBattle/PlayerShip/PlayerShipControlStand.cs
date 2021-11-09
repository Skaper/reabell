using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BNG;
public class PlayerShipControlStand : MonoBehaviour
{
    public MeshRenderer controlStandHologram;
    public MeshRenderer controlOnRightHandHologram;
    public MeshRenderer controlOnLeftHandHologram;

    public GameObject toolTip;

    public float stayDelay = 0.1f;
    private float elapsed;
    private bool canGrub = true;

    private bool isControlOnHand;

    void Start()
    {
        controlStandHologram.enabled = true;
        controlOnRightHandHologram.enabled = false;
        controlOnLeftHandHologram.enabled = false;
        isControlOnHand = false;

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft") || other.CompareTag("Player/FingerRight"))
        {
            elapsed = 0;
            canGrub = true;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft") || other.CompareTag("Player/FingerRight"))
        {
            elapsed += Time.fixedDeltaTime;
            if(canGrub && elapsed > stayDelay)
            {
                ControlStandWork();
                canGrub = false;
            }
        }
    }
   
    private void ControlStandWork()
    {
        if (!isControlOnHand)
        {
            toolTip.SetActive(false);
            controlStandHologram.enabled = false;
            controlOnRightHandHologram.enabled = true;
            controlOnLeftHandHologram.enabled = true;
            isControlOnHand = true;
        }
        else
        {
            controlStandHologram.enabled = true;
            controlOnRightHandHologram.enabled = false;
            controlOnLeftHandHologram.enabled = false;
            isControlOnHand = false;
        }
    }

    public bool isControlActive()
    {
        return isControlOnHand;
    }
}
