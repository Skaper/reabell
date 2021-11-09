using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralPointInit : MonoBehaviour
{
    private void Awake()
    {
        GameManager.WorldCenterPoint = gameObject;
    }
}
