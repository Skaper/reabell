using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHelperEp1 : MonoBehaviour
{
    public AudioClip deadEnergyLevelClip;
    public AudioClip farawayClip;

    public AudioClip a1ClipPlayerInShip;
    public AudioClip a2ClipGate1Passed;
    public AudioClip a2ClipGate4Passed;
    public AudioClip a3ClipBeaconDestroyed;
    public AudioClip a5ClipEnemiesDestroyed;
    public AudioClip a6ClipDeadShipScanned;
    public AudioClip a7ClipColossalShipFound;

    private AudioSource audioSource;

    private void Awake()
    {
        GameManager.QuestManagerEp1.AIHelper = this;
        GameManager.QuestManagerEp1.onActionPlayerShipActive += actionPlayerShipActive;
        GameManager.QuestManagerEp1.onActionGatesPassed += actionGatesPassed;
        GameManager.QuestManagerEp1.onActionBeaconDestroyed += actionBeaconDestroyed;
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed += actionEnemiesDestroyed;
        GameManager.QuestManagerEp1.onActionDeadShipScanned += actionDeadShipScanned;
        GameManager.QuestManagerEp1.onActionColossalShipFound += actionColossalShipFound;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFarawayClip()
    {
        audioSource.clip = farawayClip;
        audioSource.Play();
    }

    public void PlayDeadEnergyLevelClip()
    {
        audioSource.clip = deadEnergyLevelClip;
        audioSource.Play();
    }
    //action 1
    private void actionPlayerShipActive()
    {
        audioSource.clip = a1ClipPlayerInShip;
        audioSource.Play();
        GameManager.QuestManagerEp1.onActionPlayerShipActive -= actionPlayerShipActive;
    }
    //action 2
    private void actionGatesPassed(int gateNumber)
    {
        if(gateNumber == 2)
        {
            audioSource.clip = a2ClipGate1Passed;
            audioSource.Play();
        }
        else if (gateNumber == 4)
        {
            audioSource.clip = a2ClipGate4Passed;
            audioSource.Play();
            GameManager.QuestManagerEp1.onActionGatesPassed -= actionGatesPassed;
        }
        
    }
    //action 3
    private void actionBeaconDestroyed()
    {
        audioSource.clip = a3ClipBeaconDestroyed;
        audioSource.Play();
        GameManager.QuestManagerEp1.onActionBeaconDestroyed -= actionBeaconDestroyed;
    }

    //action 5
    private void actionEnemiesDestroyed()
    {
        audioSource.clip = a5ClipEnemiesDestroyed;
        audioSource.Play();
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed -= actionEnemiesDestroyed;
    }
    //action 6
    private void actionDeadShipScanned()
    {
        audioSource.clip = a6ClipDeadShipScanned;
        audioSource.Play();
        GameManager.QuestManagerEp1.onActionDeadShipScanned -= actionDeadShipScanned;
    }
    //action 7
    private void actionColossalShipFound()
    {
        audioSource.clip = a7ClipColossalShipFound;
        audioSource.Play();
        GameManager.QuestManagerEp1.onActionColossalShipFound -= actionColossalShipFound;
    }

}
