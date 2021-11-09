using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public int gateNumber = 0;

    private bool hasPassed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPassed)
        {
            hasPassed = true;
            GameManager.QuestManagerEp1.onActionGatesPassed?.Invoke(gateNumber);
        }
    }
}
