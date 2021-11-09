using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class MenuSpawner : MonoBehaviour
{
    public Transform menuPoint;

    private InputBridge input;
    // Start is called before the first frame update
    void Start()
    {
        input = InputBridge.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.BackButtonDown)
        {
            Debug.Log("Invoke");
            GameManager.OpenMenu?.Invoke(menuPoint);
        }
    }
}
