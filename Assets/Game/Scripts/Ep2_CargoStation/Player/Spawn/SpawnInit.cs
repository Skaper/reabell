using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInit : MonoBehaviour
{
    private void Awake()
    {
        GameManager.PlayerSpawnPoint = gameObject;
    }
}
