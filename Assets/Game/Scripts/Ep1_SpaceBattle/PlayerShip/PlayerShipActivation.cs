using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShipActivation : MonoBehaviour
{
    [Tooltip("Maximum distance which player can fly of this point")]
    public float maxDistance = 250;
    [Tooltip("Maximum distance which player can fly of this point and after that his died")]
    public float maxDeadDistance = 350;

    // public PlayerShipController shipController;
    public LevelChanger shipLeverChanger;
    public GameObject playerInShip;
    public LevelChanger introLevelChanger;
    public GameObject intro;
    public CapsuleCollider mainCollider;
    public Transform enterPoint;
    
    public ParticleSystem shipPoiter;



    private Rigidbody playerRB;
    private bool isEntering;
    private bool isPlayerInShip;
    private bool isCockpitOpen;
    private bool isAttentionSended = false;
    private PlayerInSpaceController playerInSpaceController;

    private bool playerInTrigger = false;

    public UnityEvent onDangerousDistance;
    public UnityEvent onDeadDistance;

    void Start()
    {
        playerInShip.SetActive(false);
        //shipController.isShipActive = false;
        GameManager.QuestManagerEp1.onActionPlayerShipActive += actionPlayerShipActive;
        playerInSpaceController = intro.GetComponent<PlayerInSpaceController>();
        playerRB = intro.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!isEntering)
        {
            float distance = CheckDist();
            if (!isAttentionSended && distance >= maxDistance)
            {
                isAttentionSended = true;
                onDangerousDistance?.Invoke();
            }
            if (distance > maxDeadDistance)
            {
                onDeadDistance?.Invoke();
            }
        }

        if(playerInSpaceController.isCockpitOpen && !isCockpitOpen)
        {
            isCockpitOpen = true;
        }


        if (playerInSpaceController.isPlayerInShip && !isPlayerInShip)
        {
            GameManager.QuestManagerEp1.onActionPlayerShipActive?.Invoke();
            isPlayerInShip = true;
        }


        
    }

    private void actionPlayerShipActive()
    {
        GameManager.QuestManagerEp1.onActionPlayerShipActive -= actionPlayerShipActive;
        StartCoroutine(ActivateShip());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player/PlayerInSpace") && !playerInTrigger)
        {
            playerInSpaceController.isActive = false;
            isEntering = true;
            mainCollider.isTrigger = true;
            playerRB.isKinematic = true;
            playerRB.velocity = Vector3.zero;
            playerRB.angularVelocity = Vector3.zero;
            shipPoiter.Stop();
            playerInTrigger = true;
            Debug.Log("Enter in ship");
            StartCoroutine(ActivateShip());
            
        }
    }

    

    private IEnumerator ActivateShip()
    {
        intro.GetComponent<Animator>().enabled = false;
        introLevelChanger.FadeOut();
        yield return new WaitForSeconds(3);
        Destroy(intro);
        //shipController.isShipActive = true;
        playerInShip.SetActive(true);
        yield return new WaitForSeconds(1f);
        playerInShip.GetComponent<SetCameraCenter>().Recenter();
        Destroy(gameObject);
        shipLeverChanger.Fade();
    }

    public float CheckDist()
    {
        float dist = Vector3.Distance(intro.transform.position, enterPoint.position);
        return dist;
    }
    
  
  
}
