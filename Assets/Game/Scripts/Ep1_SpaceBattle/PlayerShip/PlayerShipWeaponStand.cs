using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipWeaponStand : MonoBehaviour
{
    public MeshRenderer weaponStandHologram;
    public MeshRenderer weaponOnHandHologram;

    public GameObject toolTip;

    public float stayDelay = 0.1f;
    private float elapsed;
    private bool canGrub = true;

    private bool isWeaponOnHand;
    private void Start()
    {
        weaponStandHologram.enabled = true;
        weaponOnHandHologram.enabled = false;
        isWeaponOnHand = false;
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
            if (canGrub && elapsed > stayDelay)
            {
                WeaponStandWork();
                canGrub = false;
            }
        }
    }

    private void WeaponStandWork()
    {
        if (!isWeaponOnHand)
        {
            toolTip.SetActive(false);
            weaponStandHologram.enabled = false;
            weaponOnHandHologram.enabled = true;
            isWeaponOnHand = true;
        }
        else
        {
            weaponStandHologram.enabled = true;
            weaponOnHandHologram.enabled = false;
            isWeaponOnHand = false;
        }
    }

    public bool isWeaponActive()
    {
        return isWeaponOnHand;
    }

    
}

