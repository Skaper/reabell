using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPointController : MonoBehaviour
{


    public float speed = 0.28f;

    public float radius = 0.8f;

    private float startTime;
    private Vector3 origin;
    private float weaponReturnTime;

    private float oldXInput;
    private float oldYInput;
    void Start()
    {
        origin = transform.localPosition;

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("ZHorizontal") * 60 * Mathf.Deg2Rad;
        float y = -Input.GetAxis("ZVertical") * 60 * Mathf.Deg2Rad;

        
        x = (float)System.Math.Round(x, 3);
        y = (float)System.Math.Round(y, 3);
        if (oldXInput != x || oldYInput != y)
        {
            startTime = Time.time;
        }
        //print("JOYSTICK X:" + Input.GetAxis("ZHorizontal")  + " Y: " + Input.GetAxis("ZVertical"));

        float newX = radius * Mathf.Sin(x) * Mathf.Cos(y) + origin.x;
        float newY = radius * Mathf.Cos(x) * Mathf.Sin(y) + origin.y;
        float newZ = radius * Mathf.Cos(x) * Mathf.Cos(y) / 1.5f;// + origin.z;

        Vector3 newPos = new Vector3(newX, newY, newZ);
        float dist = Vector3.Distance(transform.localPosition, newPos);
        if(dist > 0.001f)
        {
            float distCovered = (Time.time - startTime) * speed;

            float fractionOfJourney = distCovered / dist;


            transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, fractionOfJourney); //newPos;
        }
        
        oldXInput = x;
        oldYInput = y;
    }
   
}
