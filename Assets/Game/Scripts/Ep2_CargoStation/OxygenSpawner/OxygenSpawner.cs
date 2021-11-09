using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenSpawner : MonoBehaviour
{
    public GameObject oxygenTank;
    public Transform spawnPoint;
    public float spawnDelayMin = 45;
    public float spawnDelayMax = 60;


    public bool isStart = false;

    private float currentSpawnTimeDelay;
    private float spawnTimer;
    private GameObject currentOxygentTank = null;

    private AudioSource audioSource;


    void Start()
    {
        GameManager.QuestManagerEp2.OnActionAirCompressor_off += OnActionAirCompressor_off;
        audioSource = GetComponent<AudioSource>();
    }
    private void OnActionAirCompressor_off()
    {
        isStart = true;
        GameManager.QuestManagerEp2.OnActionAirCompressor_off -= OnActionAirCompressor_off;
    }
    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            
            if(currentOxygentTank == null && spawnTimer >= currentSpawnTimeDelay)
            {
                Spawn();
            }
            spawnTimer += Time.deltaTime;
            if(currentOxygentTank != null && Vector3.Distance(transform.position, currentOxygentTank.transform.position) > 2f)
            {
                currentOxygentTank = null;
            }
        }
    }

    private void Spawn()
    {
        spawnTimer = 0;
        currentOxygentTank = Instantiate(oxygenTank, spawnPoint.position, transform.rotation);
        currentSpawnTimeDelay = Random.Range(spawnDelayMin, spawnDelayMax);
        if (!audioSource.isPlaying) audioSource.Play();
    }
}
