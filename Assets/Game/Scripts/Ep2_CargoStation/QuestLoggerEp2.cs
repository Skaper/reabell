using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLoggerEp2 : MonoBehaviour
{
    public QuestLogWindow logWindow;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.QuestManagerEp2.OnAction_LevelStart += QuestManager_OnAction_LevelStart;

        
    }

    public void MakeLog(string clipName)
    {
        string logText = "";
        switch (clipName)
        {
            case "ai_ep2_m1":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_m1");
                break;
            case "ai_e1":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e1");
                break;
            case "ai_e2":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e2");
                break;
            case "ai_e3":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e3");
                break;
            case "ai_e3_1":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e3_1");
                break;
            case "ai_e3_2":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e3_2");
                break;
            case "ai_e3_3":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e3_3");
                break;
            case "ai_e5":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e5");
                break;
            case "ai_e6":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e6");
                break;
            case "ai_e_b1":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e_b1");
                break;
            case "ai_e_b2":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e_b2");
                break;
            case "ai_e_b3":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e_b3");
                break;
            case "ai_e7":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e7");
                break;
            case "ai_e8":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e8");
                break;
            case "ai_e9":
                logText = AIDictionaty.GetText("en-US-C_ai_ep2_e9");
                break;
        }
        logWindow.MakeLog?.Invoke(logText);
    }
    private void QuestManager_OnAction_LevelStart()
    {
        logWindow.MakeLog?.Invoke(AIDictionaty.GetText("en-US-C_ai_ep2_m1"));
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
