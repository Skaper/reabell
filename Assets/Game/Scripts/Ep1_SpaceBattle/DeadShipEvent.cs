using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadShipEvent : MonoBehaviour
{
    public PlayerShipScanner shipScanner;
    private ScannerData scannerData;

    void Start()
    {
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed += OnEnemiesDestroyed;
        if (shipScanner != null)
        {
            scannerData = GetComponent<ScannerData>();
            shipScanner.onDataIncome += OnIncomeData;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnIncomeData(string data)
    {
        if (scannerData.Data.Equals(data))
        {
            GameManager.QuestManagerEp1.onActionDeadShipScanned?.Invoke();
            shipScanner.onDataIncome -= OnIncomeData;
        }
        
    }

    private void OnEnemiesDestroyed()
    {
        GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(transform);
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed -= OnEnemiesDestroyed;
    }


}
