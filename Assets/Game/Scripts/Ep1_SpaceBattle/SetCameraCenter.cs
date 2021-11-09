using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
public class SetCameraCenter : MonoBehaviour
{
    public Transform floorPos;
    public Transform playerHead;
    public Transform floorOffset;
    public float maxOffset = 1f;
    private InputBridge input;

    public bool recenterOnStart = false;

    void Start()
    {

        input = InputBridge.Instance;
        if (recenterOnStart)
            Recenter();
        Debug.Log("RECENTER CAMERA ON START");
    }


    // Update is called once per frame
    void Update()
    {
        if (input.RightThumbstickDown || input.LeftThumbstickDown)
        {
            Recenter();
        }
        //if (Application.platform == RuntimePlatform.Android && input.RightThumbNear)
        //{
        //
        //}else if(input.RightThumbstickDown || input.LeftThumbstickDown)
        //{
        //    Recenter();
        //}
    }

    public void Recenter()
    {
        float dist = Mathf.Abs((playerHead.position - floorPos.position).y);
        if (dist > maxOffset)
        {
            //floorOffset.localPosition = new Vector3(0, -dist + maxOffset, 0);
            transform.localPosition = new Vector3(transform.localPosition.x, -dist + maxOffset, transform.localPosition.z) ;
        }
        //if (Application.platform != RuntimePlatform.Android) 
        InputTracking.Recenter();
        Debug.Log("Recenter character, dist: " + dist);
    }
}
