using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float oxygenStartLevel = 30f;
    public float oxygenStartMaxLevel = 60f;
    public float oxygenStartTick = 0.1f;

    public float healthStartTick = 0.1f;

    public float OxygenLevel { get; private set; }
    public float OxygenMax { get; private set; }

    public float OxygenTick { get; set; }
    public float HealthTick { get; private set; }

    private void Awake()
    {
        OxygenLevel = oxygenStartLevel;
        OxygenMax =   oxygenStartMaxLevel;
        OxygenTick =  oxygenStartTick;
        HealthTick = healthStartTick;
    }

    public void AddOxygen()
    {
        OxygenLevel = OxygenMax;
    }

    public void AddOxygen(float value)
    {
        OxygenLevel += value;
        if (OxygenLevel > OxygenMax) OxygenLevel = OxygenMax;
    }

    public void TickOxygen()
    {
        OxygenLevel -= OxygenTick;
    }

    

}
