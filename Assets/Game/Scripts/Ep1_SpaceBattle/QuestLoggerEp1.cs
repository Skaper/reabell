using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class QuestLoggerEp1 : MonoBehaviour
{
    public QuestLogWindow logWindow;
    
    void Start()
    {
        GameManager.QuestManagerEp1.onActionPlayerShipActive += actionPlayerShipActive;
        GameManager.QuestManagerEp1.onActionGatesPassed += actionGatesPassed;
        GameManager.QuestManagerEp1.onActionEnemiesAwakened += actionEnemiesAwake;
        GameManager.QuestManagerEp1.onActionEnemiesDestroyed += actionEnemiesDestroyed;
        GameManager.QuestManagerEp1.onActionDeadShipScanned += actionDeadShipScanned;
        GameManager.QuestManagerEp1.onActionColossalShipFound += actionColossalShipFound;
    }

    private void actionPlayerShipActive()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_e1"));
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_a1"));
    }

    private void actionGatesPassed(int gateNumber)
    {
        if(gateNumber==4) logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_a2b"));
    }

    private void actionEnemiesAwake()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_a3"));
    }

    private void actionEnemiesDestroyed()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_a5"));
    }

    private void actionDeadShipScanned()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_a6"));
    }

    private void actionColossalShipFound()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep1_a7"));
    }
}
