using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Player = gameObject;
    }
}
