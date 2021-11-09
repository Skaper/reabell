using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugConsole : MonoBehaviour
{

    public Text output;
    static string myLog = "";
    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }
    public void Log(string logString, string stackTrace, LogType type)
    {
        string outputText = logString;
        string stack = stackTrace;
        myLog = outputText + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
        output.text = myLog;
    }
}
