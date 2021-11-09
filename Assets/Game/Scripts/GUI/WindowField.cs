using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowField : MonoBehaviour
{
    public bool isWindowActive { get; private set; }
    private void Start()
    {
        gameObject.SetActive(false);
        isWindowActive = false;
    }

    public void ShowWindow()
    {
        gameObject.SetActive(true);
        isWindowActive = true;
    }

    public void HideWindow()
    {
        gameObject.SetActive(false);
        isWindowActive = false;
    }


}
