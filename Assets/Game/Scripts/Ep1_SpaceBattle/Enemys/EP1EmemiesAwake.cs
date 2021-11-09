using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EP1EmemiesAwake : MonoBehaviour
{
    public GameObject[] enemies;
    public ParticleSystem[] waprFXs;

    private int shipDies = 0;

    private bool isEnemiesAwaked;
    private bool isEnemiesDestroyed;
    void Start()
    {
        GameManager.QuestManagerEp1.onActionEnemiesAwakened += onEnemiesAwake;
        foreach(GameObject go in enemies)
        {
            go.SetActive(false);
            EnemyShipAI enemy = go.GetComponent<EnemyShipAI>();
            enemy.onDead += onShipDied;
            enemy.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemiesAwaked && enemies.Length == shipDies && !isEnemiesDestroyed)
        {
            GameManager.QuestManagerEp1.onActionEnemiesDestroyed?.Invoke();
            isEnemiesDestroyed = true;
            Debug.Log("EVENT > Enemies has EnemiesDestroyed!");
            Destroy(gameObject, 4f);
            
        }
    }

    private void onEnemiesAwake()
    {
        foreach (GameObject go in enemies)
        {
            go.SetActive(true);
            EnemyShipAI enemy = go.GetComponent<EnemyShipAI>();
            enemy.enabled = true;
            
        }

        foreach (ParticleSystem ps in waprFXs)
        {
            if (!ps.isPlaying) ps.Play();
            Destroy(ps.gameObject, 4f);
        }

        GameManager.QuestManagerEp1.onActionEnemiesAwakened -= onEnemiesAwake;
        isEnemiesAwaked = true;
        Debug.Log("EVENT > Enemies has awakened!");
    }

    private void onShipDied(EnemyShipAI enemy)
    {
        Debug.Log("Enemy destroyed №" + shipDies + ": " + enemy.gameObject.name);
        enemy.onDead -= onShipDied;
        shipDies++;
    }
}
