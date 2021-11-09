using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraInit : MonoBehaviour
{
    private void Awake()
    {
        GameManager.PlayerCamera = gameObject;
    }
}
