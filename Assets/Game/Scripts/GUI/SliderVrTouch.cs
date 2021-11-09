using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;
using TMPro;
public class SliderVrTouch : MonoBehaviour
{
    public TextMeshProUGUI valueText;

    public Transform Left;
    public Transform Right;

    public float value { get; private set; }

    private float leftRightDist;

    private InputBridge input;



    void Start()
    {
        input = InputBridge.Instance;

        leftRightDist = Vector3.Distance(Left.position, Right.position);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft") || other.CompareTag("Player/FingerRight"))
        {
            Vector3 projectionPoint = Vector3.Project((other.transform.position - Left.position), (Right.position - Left.position)) + Left.position;
            float distL = Vector3.Distance(Left.position, projectionPoint);
            
            if(projectionPoint.x < Left.position.x)
            {
                transform.position = Left.position;
            }else if(projectionPoint.x > Right.position.x)
            {
                transform.position = Right.transform.position;
            }
            else
            {
                value = mapConvert(distL, 0f, leftRightDist, 0f, 1f);
                if (value > 1) value = 1;
                else if (value < 0) value = 0;
                valueText.text = Mathf.Round(value * 100f) + "";
                transform.position = projectionPoint;
            }
                
            if (other.CompareTag("Player/FingerLeft"))
            {
                input.VibrateController(0.1f, 0.1f, 0.1f, ControllerHand.Left);

            }
            else if (other.CompareTag("Player/FingerRight"))
            {
                input.VibrateController(0.1f, 0.1f, 0.1f, ControllerHand.Right);
            }
        }
    }


    float mapConvert(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public void SetValue(float _value)
    {
        value = _value;
        if (value > 1f) value = 1;
        else if (value < 0f) value = 0;

        float lrXDist = Right.localPosition.x - Left.localPosition.x;
        float dist = mapConvert(_value, 0f, 1f, 0f, lrXDist);
        
        valueText.text = Mathf.Round(value * 100f) + "";
        transform.localPosition = new Vector3(Left.localPosition.x + dist, 0f, 0f);
    }
}
