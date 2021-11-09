using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPointInit : MonoBehaviour
{
    private void Awake()
    {
        GameManager.PlayerMenuPoint = gameObject;
    }
}
