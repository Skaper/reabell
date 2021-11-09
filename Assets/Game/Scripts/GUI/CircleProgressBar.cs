using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleProgressBar : MonoBehaviour
{
    
    private SpriteRenderer renderer;
    private Material material;
    public Camera camera;

    public int value = 10;
    public int maxValue = 100;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        //camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        float newValue = Map(value, 0, maxValue, 180, 0);
        renderer.sharedMaterial.SetInt("_Value", (int)newValue);

        Vector3 camAngle = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z);
        Quaternion deltaRotation = Quaternion.Euler(camAngle);
        //gameObject.transform.rotation = deltaRotation;
    }

    public void setValue(int value)
    {

    }

    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        if (x < in_min) return out_min;
        if (x > in_max) return out_max;
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
