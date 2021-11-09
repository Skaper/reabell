using UnityEngine;
using System.Collections.Generic;
using System;
public class JoystickControllSetup : MonoBehaviour
{
    [Tooltip("Is joystick active in Windows")]
    public bool isJoystickActive = false;

    private bool isJoystickSetup = false;
    private bool isKeyboardAttached = false;

    private float pressTimeOut = 0.15f;
    private float timer;

    private bool isCompliteSetup = false;

    private Dictionary<InputManager.KeyType, string> joystickKeys;
    void Awake()
    {
        GameManager.joystickControll = this;
        InputManager.OnActionSelectingKey += OnActionSelectingKey;
        InputManager.OnActionSaveAllKeys += SaveJoystickPref;
        InputManager.OnActionReloadAllKeys += LoadJoystickPref;
        joystickKeys = new Dictionary<InputManager.KeyType, string>();
        
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            InputManager.setJoystickActive(isJoystickActive);
            if(isJoystickActive) LoadJoystickPref();
        }
        else
        {
            LoadJoystickPref();
        }
            
    }

    private void SetKey(InputManager.KeyType keyType, JoystickButton.TextDelegate textDekegate)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isKeyboardAttached = true;
        }

        timer += Time.deltaTime;
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode) && timer > pressTimeOut)
            {
                string keyName = kcode.ToString();
                setJoystickKey(keyType, keyName);

                if (joystickKeys.ContainsKey(keyType)) joystickKeys[keyType] = keyName;
                else joystickKeys.Add(keyType, keyName);

                if (textDekegate != null)
                {
                    textDekegate?.Invoke(keyName);
                }
                timer = 0;
                if (isCompliteSetup)
                {
                    break;
                }
            }
        }


        
        
    }

    private void OnActionSelectingKey(InputManager.KeyType keyType, JoystickButton.TextDelegate textDekegate)
    {
        SetKey(keyType, textDekegate);
    }

    //public void SetKeyManually(InputManager.KeyType keyType, string joystickKeyName)
    //{
    //    SetKey(keyType, null);
    //    InputManager.setJoystickActive(true);
    //    setJoystickKey(keyType, joystickKeyName);
    //    //string buttonText = setupJoystick(n, i, keyType); //int joystickNumber, int joystickKeyNumber
    //}

    private void SaveJoystickPref()
    {
        if (joystickKeys.Count == 0) return;

        PlayerPrefs.SetString("Joystick","yes");
        foreach (var item in joystickKeys)
        {
            PlayerPrefs.SetString(item.Key.ToString(), item.Value);
        }

        PlayerPrefs.Save();
    }

    private void LoadJoystickPref()
    {
        if (!PlayerPrefs.HasKey("Joystick")) return;
        foreach(InputManager.KeyType key in (InputManager.KeyType[])System.Enum.GetValues(typeof(InputManager.KeyType)))
        {
            if (!PlayerPrefs.HasKey(key.ToString())) continue;
            string value = PlayerPrefs.GetString(key.ToString());
            setJoystickKey(key, value);
            //value = value.Replace("Joystick", "J").Replace("Button", "B");
            InputManager.OnActionLoadKey?.Invoke(key, value);
        }
    }
    

    private void setJoystickKey(InputManager.KeyType keyType, string joystickKeyName)
    {
        switch (keyType)
        {
            case InputManager.KeyType.Jump:
                InputManager.Jump = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Attack1:
                InputManager.Attack1 = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Attack2:
                InputManager.Attack2 = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Action1:
                InputManager.Action1 = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Action2:
                InputManager.Action2 = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Action3:
                InputManager.Action3 = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Action4:
                InputManager.Action4 = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Next:
                InputManager.Next = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Previous:
                InputManager.Previous = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Sit:
                InputManager.Sit = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Select:
                InputManager.Select = (KeyCode)System.Enum.Parse(typeof(KeyCode),
                    joystickKeyName);
                break;
            case InputManager.KeyType.Exit:
                isCompliteSetup = true;
                break;
        }

    }

}
