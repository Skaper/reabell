using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class QuestLogWindow : MonoBehaviour
{
    public TextMeshProUGUI textLog;
    public Action<string> MakeLog;
    void Start()
    {
        MakeLog += doLog;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void doLog(string text)
    {
        textLog.text = text + "\n" + "---" + "\n" + textLog.text;
    }
}
