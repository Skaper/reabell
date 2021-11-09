using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLoggerEp2_1 : MonoBehaviour
{
    public QuestLogWindow logWindow;

    bool isDronesActive = false;
    void Start()
    {
        
    }

    public void Action_LevelStart()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep2_1_c8"));
    }

    public void Action_Drones()
    {
        if (!isDronesActive)
        {
            logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep2_1_c9"));
            isDronesActive = true;
        }
        
    }

    public void Action_Block()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep2_1_c10"));
    }




}
