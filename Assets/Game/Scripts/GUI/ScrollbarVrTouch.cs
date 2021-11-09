using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;
public class ScrollbarVrTouch : MonoBehaviour
{

    public CapsuleCollider handleCollider;
    public Scrollbar scrollbar;
    public Transform Up;
    public Transform Down;

    private float handleHeight;
    private float scrollBarSize;

    private float upDownDist;

    private InputBridge input;

    void Start()
    {
        input = InputBridge.Instance;
        handleHeight = 54;
        scrollBarSize = scrollbar.size;
        handleCollider.height = handleHeight * scrollBarSize;

        upDownDist = Vector2.Distance(Up.position, Down.position);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (scrollBarSize != scrollbar.size)
        {
            scrollBarSize = scrollbar.size;
            handleCollider.height = handleHeight * scrollBarSize + 20f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player/FingerLeft") || other.CompareTag("Player/FingerRight"))
        {
            Vector3 projectionPoint = Vector3.Project((other.transform.position - Up.position), (Down.position - Up.position)) + Up.position;
            float dist = Vector3.Distance(Up.position, projectionPoint);
            float value = mapConvert(dist, 0f, upDownDist, 1.1f, 0f);
            if (value > 1) value = 1;
            else if (value < 0) value = 0;
            scrollbar.value = value;

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
}
