using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ep1ColossalShipEvent : MonoBehaviour
{
    public Transform EnterPoint;
    private bool hasSendSingnal;
    private Animator animator;
    public Light[] lights; 
    void Start()
    {
        
        animator = GetComponent<Animator>();
        GameManager.QuestManagerEp1.onActionDeadShipScanned += OnDeadShipScaned;
    }
    private void OnDeadShipScaned()
    {
        GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(EnterPoint);
        GameManager.QuestManagerEp1.onActionDeadShipScanned -= OnDeadShipScaned;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!hasSendSingnal && other.gameObject.CompareTag("Player"))
        {
            animator.Play("OpenMainDoor");
            GameManager.QuestManagerEp1.onActionColossalShipFound?.Invoke();
            foreach (Light l in lights)
            {
                l.enabled = true;
            }
        }
    }
}
