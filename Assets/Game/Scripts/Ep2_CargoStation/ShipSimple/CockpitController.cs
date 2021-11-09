using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CockpitController : MonoBehaviour
{
    public AudioClip soundEffect;
    public ParticleSystem moveFX;
    public Battlehub.SplineEditor.Spline outPath;
    public Transform openPosition;
    public Transform closePosition;
    
    public MeshCollider cockpitCollider;
    public Transform cameraPoint;
    public float cameraAttractSpeed = 2f;

    public float speed = 50f;

    public bool closing = false;
    public bool startClosing = false;
    public bool opening = false;
    private bool playerIn = false;
    public bool isChargingComlite = false;
    public UnityEvent onStartFly;
    public UnityEvent onPlayerInCockpit;

    private float closeDelay = 3f;
    private float cloaseTimer;

    private AudioSource audio;

    public Battlehub.SplineEditor.ShipMoving shipMovingScript;
    private GameObject player;
    private float levelEndTimer;
    private bool levelFinished = false;
    public float levelEndingDelay = 10f;
    public UnityEvent levelComplite;

    private bool playerMoving = true;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.QuestManagerEp2.OnActionTowerCrane_ChargingComplited += openCockpit;
        player = GameManager.Player;
        audio = GetComponent<AudioSource>();
        audio.loop = false;
        audio.playOnAwake = false;
        audio.clip = soundEffect;
        moveFX.Stop();
        //shipMovingScript = GetComponentInParent<Battlehub.SplineEditor.ShipMoving>();
  
    }

    // Update is called once per frame
    void Update()
    {
        if (opening)
        {
            transform.transform.rotation = Quaternion.RotateTowards(
            transform.transform.rotation,
            openPosition.rotation,
            Time.deltaTime * speed
            );
            if (Quaternion.Angle(transform.rotation, openPosition.rotation) < 0.05f) opening = false;
            cockpitCollider.enabled = false;
        }

        if (closing)
        {
            if(cloaseTimer >= closeDelay)
            {
                startClosing = true;
            }
            cloaseTimer += Time.deltaTime;
            if (startClosing)
            {
                transform.transform.rotation = Quaternion.RotateTowards(
                    transform.transform.rotation,
                    closePosition.rotation,
                    Time.deltaTime * speed
                    );
                if (Quaternion.Angle(transform.rotation, closePosition.rotation) < 0.05f)
                {
                    audio.PlayOneShot(soundEffect);
                    closing = false;
                    startClosing = false;
                    cockpitCollider.enabled = true;

                    shipMovingScript.Spline = outPath;
                    shipMovingScript.IsRunning = true;
                    if (!moveFX.isPlaying) moveFX.Play();
                    levelFinished = true;
                }
            }
            
        }
        if (playerIn && isChargingComlite)
        {
           if(playerMoving && Vector3.Distance(cameraPoint.position, player.transform.position) > 0.08f)
            {

                player.transform.SetParent(cameraPoint);
                player.transform.position = Vector3.Lerp(
                    player.transform.position,
                    cameraPoint.position,
                    cameraAttractSpeed * Time.deltaTime
                    );
                
            }
            else
            {
                player.transform.position = cameraPoint.position;
                playerMoving = false;
                onStartFly.Invoke();
            }
        }
        if (levelFinished)
        {
            if(levelEndTimer >= levelEndingDelay)
            {
                levelComplite.Invoke();
                levelFinished = false;
            }
            levelEndTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("IN");
            CMF.AdvancedWalkerController awc = GameManager.Player.GetComponent<CMF.AdvancedWalkerController>();
            if (awc != null)
            {
                player.GetComponent<PlayerTeleport>().enabled = false;
                other.attachedRigidbody.velocity = Vector3.zero;
                awc.enabled = false;
                closing = true;
                opening = false;
                playerIn = true;
                onPlayerInCockpit.Invoke();
                
            }

        }
    }

    private void openCockpit()
    {
        opening = true;
        isChargingComlite = true;
        audio.Play();
        GameManager.QuestManagerEp2.OnActionTowerCrane_ChargingComplited -= openCockpit;
    }
}
