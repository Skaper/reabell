using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSettings : MonoBehaviour
{

    public Color fogColor = Color.blue;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fog = true;
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.fogColor = fogColor;

        // And enable fog
        
    }
}
