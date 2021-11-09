using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;

public class BeaconController : vHealthController
{
    [vEditorToolbar("Settings", order = 200)]
    public GameObject beacon;
    public ParticleSystem explosionFX;
    public ParticleSystem beaconFX;

    private MeshRenderer meshRenderer;
    private bool isTarget;
    private bool hasSendSignal;
    void Start()
    {
        base.Start();
        meshRenderer = beacon.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isTarget && GameManager.QuestManagerEp1.GatePassedNumber == 4)
        {
            GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(transform);
            isTarget = true;
        }
        if (isDead && !hasSendSignal)
        {
            GameManager.QuestManagerEp1.onActionBeaconDestroyed?.Invoke();
            GameManager.QuestManagerEp1.onActionEnemiesAwakened?.Invoke();
            hasSendSignal = true;
        }

    }
    private IEnumerator DestroySequence()
    {
        if(!explosionFX.isPlaying) explosionFX.Play();
        meshRenderer.enabled = false;
        beaconFX.Stop();
        while (explosionFX.isPlaying && !hasSendSignal)
        {
            yield return null;
        }

        Destroy(gameObject, 2f);
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bullet"))
        {
            float damage = other.gameObject.GetComponent<SimpleBullet>().damage;
            currentHealth -= damage;
            if (currentHealth <= 0)
                StartCoroutine(DestroySequence());
        }
    }
}
