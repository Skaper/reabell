using System;
using UnityEngine;

public class JoystickMainKeysSetup : MonoBehaviour
{
    public GameObject selectKey;
    public GameObject startKey;

    private bool isSelectSetup;
    private bool isStartSetup;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                if (!isSelectSetup)
                {
                    Debug.Log("KeyCode select key: " + kcode);
                    //GameManager.joystickControll.SetKeyManually()
                }
            }
                
        }
    }
}
