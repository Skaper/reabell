using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologrammPointInit : MonoBehaviour
{
    private void Awake()
    {
        GameManager.PlayerHologrammPoint = gameObject;
    }
}
