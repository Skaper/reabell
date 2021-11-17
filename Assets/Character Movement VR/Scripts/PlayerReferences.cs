using UnityEngine;
using CMF;
using Invector;

public class PlayerReferences : MonoBehaviour
{
    [Header("ENABLE COMPONENTS")]
    public bool ActiveWalkerController = true;
    public bool ActivePlayerClimb = true;
    public bool ActivePlayerTeleport = true;
    [Space]
    [Header("COMPONENTS")]
    public GameObject Head;
    public GameObject LeftHand;
    public GameObject RightHand;
    public PlayerHealthController PlayerHealthController;
    public CapsuleCollider BodyCollider;
    public LevelChanger LevelChanger;
    public Vector3 CharacterOffset = new Vector3(0f, -0.8f, 0f);
    public Rigidbody RigRigidbody { get; private set; }
    public PlayerClimb PlayerClimb { get; private set; }
    public AdvancedWalkerController PlayerWalkerController { get; private set; }
    
    public PlayerTeleport PlayerTeleport { get; private set; }
    
    private void Awake()
    {
        GameManager.PlayerReferences = this;
        RigRigidbody = GetComponent<Rigidbody>();
        PlayerClimb = GetComponent<PlayerClimb>();
        PlayerWalkerController = GetComponent<AdvancedWalkerController>();
        PlayerTeleport = GetComponent<PlayerTeleport>();

        if (!ActivePlayerClimb)
        {
            PlayerClimb.enabled = false;
        }

        if (!ActiveWalkerController)
        {
            PlayerWalkerController.enabled = false;
        }

        if (!ActivePlayerTeleport)
        {
            PlayerTeleport.enabled = false;
        }
        
    }
}
