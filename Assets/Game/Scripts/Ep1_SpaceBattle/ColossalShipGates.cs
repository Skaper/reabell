using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColossalShipGates : MonoBehaviour
{
    public PlayerShipController playerShip;
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerShip.backRecoilForce = 0.5f;
        }
    }
}
