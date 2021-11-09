using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallTerminal : MonoBehaviour
{

    public GameObject antenaDisk;
    public float diskSpeed = 50f;
    public ParticleSystem guiParticle;


    private AudioQueue audioQueue;
    private bool isButtonPresed = false;
    private bool isTerminalOn = false;

    // Start is called before the first frame update
    void Start()
    {
        guiParticle.Stop();
        audioQueue = GetComponent<AudioQueue>();
        GameManager.QuestManagerEp2.OnActionShip_came += QuestManager_OnActionShip_came;

    }

    // Update is called once per frame
    void Update()
    {

        if (isTerminalOn)
        {
            antenaDisk.transform.Rotate(Vector3.up, diskSpeed * Time.deltaTime);
        }
    }

    public void Use()
    {
        if (GameManager.QuestManagerEp2.towerCrane_batteryCount < 3)
        {
            GameManager.AIHelper.GetComponent<AIHelper>().Play("ai_e6");
        }
        else if (!isTerminalOn)
        {
            guiParticle.Play();
            audioQueue.Play();
            isButtonPresed = true;
            isTerminalOn = true;
            GameManager.QuestManagerEp2.OnActionCallTerminall_on?.Invoke();
        }
    }

    private void QuestManager_OnActionShip_came()
    {
        audioQueue.Stop();
    }

}
