using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCraneWork : MonoBehaviour
{
    public float speed = 5f;
    public Transform startPoint;
    public Transform endPoint;
    public GameObject grab;
    public GameObject grabLight;
    public GameObject rope;

    public AudioClip workStartSound;
    public AudioClip workLoopSound;


    private AudioSource audioSource;

    private bool isMoving = false;
    private bool isStart = false;
    private bool isCharging = false;
    private bool isMovingBack = false;
    private float chargingTime;
    private float currentChargingTime;
    void Start()
    {
        grab.transform.position = startPoint.position;
        rope.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        rope.transform.position = startPoint.position;

        chargingTime = QuestManagerEp2.ChargingTime;
        GameManager.QuestManagerEp2.OnActionControlPanel_access += QuestManager_OnActionControlPanel_access;

        audioSource = GetComponent<AudioSource>();


    }
    private int batteryCount = 0;
    private bool shipCome = false;



    private void QuestManager_OnActionControlPanel_access()
    {
        isStart = true;
        audioSource.clip = workStartSound;
        audioSource.Play();
        audioSource.loop = false;
        GameManager.QuestManagerEp2.OnActionControlPanel_access -= QuestManager_OnActionControlPanel_access;

    }

    void Update()
    {
        if (isStart)
        {
            if (!audioSource.isPlaying)
            {
                isStart = false;
                isMoving = true;
                audioSource.clip = workLoopSound;
                audioSource.Play();
                audioSource.loop = true;
            }
        }
        if (isMoving)
        {
            if (!moveGrab(endPoint.position))
            {
                grabLight.active = true;
                GameManager.QuestManagerEp2.OnActionTowerCrane_Grab?.Invoke();
                isMoving = false;
                isCharging = true;
            }
        }
        if (isCharging)
        {
            currentChargingTime += Time.deltaTime;
            if (currentChargingTime >= chargingTime)
            {
                isCharging = false;
                grabLight.active = false;
                isMovingBack = true;
                GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e9");
                GameManager.QuestManagerEp2.OnActionTowerCrane_ChargingComplited?.Invoke();
            }
        }
        if (isMovingBack)
        {
            if (!moveGrab(startPoint.position))
            {
                isMovingBack = false;
                audioSource.Stop();
                isStart = false;
                
            }
        }
    }

    private bool moveGrab(Vector3 position)
    {
        if (Vector3.Distance(grab.transform.position, position) > 0.05f)
        {
            float ropeLen = Vector3.Distance(startPoint.position, grab.transform.position);
            rope.transform.position = new Vector3(rope.transform.position.x, startPoint.position.y - ropeLen / 2f, rope.transform.position.z);
            rope.transform.localScale = new Vector3(rope.transform.localScale.x, ropeLen, rope.transform.localScale.z);
            grab.transform.position = Vector3.MoveTowards(grab.transform.position, position, speed * Time.deltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }
}
