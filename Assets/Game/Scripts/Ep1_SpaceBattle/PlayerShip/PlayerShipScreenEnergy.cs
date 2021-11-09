using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShipScreenEnergy : MonoBehaviour
{
    public PlayerShipController shipController;
    public PlayerShipScanner shipScanner;

    [Header("Motion info screen")]
    public GameObject MotionInfoScreen;
    public GameObject energyProgress;
    public GameObject speedProgress;
    public Color dangerouColor;

    [Header("Scanner info screen")]
    public GameObject ScannerInfoScreen;
    public GameObject scannerData;

    private TextMeshProUGUI scannerText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI energyText;

    
    

    private Color normalColor;
    void Start()
    {
        energyText = energyProgress.GetComponent<TextMeshProUGUI>();
        speedText = speedProgress.GetComponent<TextMeshProUGUI>();
        scannerText = scannerData.GetComponent<TextMeshProUGUI>();

        MotionInfoScreen.SetActive(true);
        ScannerInfoScreen.SetActive(false);

        normalColor = energyText.color;
        if (shipScanner != null)
        {
            shipScanner.onDataIncome += onIncomeData;
            shipScanner.onScannerChangedState += onScannerChangedState;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 5 == 0)
        {
            float energyPercent = 
                shipController.GetEnergyLevel() / shipController.GetMaxEnergyLevel() * 100;
            energyText.SetText(Mathf.RoundToInt(energyPercent) + "%");
            if (energyPercent < 30f) energyText.color = dangerouColor;
            else energyText.color = normalColor;

            float speedPercent = shipController.GetThrust() / shipController.GetMaxThrust() * 100;
            speedText.SetText(Mathf.RoundToInt(speedPercent) + "%");
        }
    }

    private void onIncomeData(string data)
    {
        scannerText.SetText(data);
    }

    private void onScannerChangedState(bool state)
    {
        if (state)
        {
            MotionInfoScreen.SetActive(false);
            ScannerInfoScreen.SetActive(true);

        }
        else
        {
            MotionInfoScreen.SetActive(true);
            ScannerInfoScreen.SetActive(false);
        }
    }

}
