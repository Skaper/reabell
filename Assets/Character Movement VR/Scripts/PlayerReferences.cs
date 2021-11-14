using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;
using Invector;

public class PlayerReferences : MonoBehaviour
{
    public GameObject head;
    public GameObject leftHand;
    public GameObject rightHand;
    public PlayerHealthController playerHealthController;
    public CapsuleCollider BodyCollider;
    public LevelChanger levelChanger;
    public Vector3 characterOffset = new Vector3(0f, -0.8f, 0f);
    public AdvancedWalkerController advancedWalkerController;
    public Rigidbody rigRigidbody { get; private set; }
    public PlayerClimb playerClimb { get; private set; }
    public AdvancedWalkerController playerWalk { get; private set; }
    
    private void Awake()
    {
        GameManager.PlayerReferences = this;
        rigRigidbody = GetComponent<Rigidbody>();
        playerClimb = GetComponent<PlayerClimb>();
        playerWalk = GetComponent<AdvancedWalkerController>();
        if (playerWalk == null) playerWalk = advancedWalkerController;
    }
}
