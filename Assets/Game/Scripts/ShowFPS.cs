using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowFPS : MonoBehaviour
{
    private Text textShow;

    void Start()
    {
        textShow = GetComponent<Text>();
    }

    public string formatedString = "FPS {value}";

    public float updateRateSeconds = 1.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0F;

    void Update()
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRateSeconds)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0F / updateRateSeconds;
        }
        textShow.text = formatedString.Replace("{value}", System.Math.Round(fps, 1).ToString("0.0"));
    }
}
