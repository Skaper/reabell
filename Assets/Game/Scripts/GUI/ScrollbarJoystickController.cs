using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollbarJoystickController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Scrollbar horizontalScrollbar;
    public Scrollbar verticalScrollbar;
    public float sensitivity = 0.5f;
    public float scrollSpeed = 1f;
    [Range(0f, 1f)]
    public float startValue = 0f;

    public bool horizontalMove = false;
    public bool verticalMove = true;

    public string horizontalName = "ZHorizontal";
    public string verticalName = "ZVertical";


    private bool inFocus;
    public void OnPointerEnter(PointerEventData eventData)
    {
        inFocus = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inFocus = false;
    }

    void Start()
    {
        if (verticalMove)
        {
            verticalScrollbar.value = startValue;
        }
        if (horizontalMove)
        {
            horizontalScrollbar.value = startValue;
        }
    }

    void Update()
    {
        if (inFocus)
        {
            if (verticalMove)
            {
                float roll = -Input.GetAxis(verticalName);
                if (roll < -sensitivity)
                {
                    verticalScrollbar.value -= scrollSpeed * Time.deltaTime;
                    if (verticalScrollbar.value < 0f) verticalScrollbar.value = 0f;
                }
                else if (roll > sensitivity)
                {
                    verticalScrollbar.value += scrollSpeed * Time.deltaTime;
                    if (verticalScrollbar.value > 1f) verticalScrollbar.value = 1f;
                }
            }
            if (horizontalMove)
            {
                float roll = Input.GetAxis(horizontalName);
                if (roll < -sensitivity)
                {
                    horizontalScrollbar.value -= scrollSpeed * Time.deltaTime;
                    if (horizontalScrollbar.value < 0f) horizontalScrollbar.value = 0f;
                }
                else if (roll > sensitivity)
                {
                    horizontalScrollbar.value += scrollSpeed * Time.deltaTime;
                    if (horizontalScrollbar.value > 1f) horizontalScrollbar.value = 1f;
                }
            }

        }
        
    }
}
