using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public static class InputManager
{
    public static Action<KeyType, JoystickButton.TextDelegate> OnActionSelectingKey;
    
    public static Action<KeyType, string> OnActionLoadKey;

    public static Action OnActionSaveAllKeys;
    public static Action OnActionReloadAllKeys;
    public enum KeyType
    {
        None = 0,
        Jump = 1,
        Attack1 = 2,
        Attack2 = 3,
        Action1 = 4,
        Action2 = 5,
        Action3 = 6,
        Action4 = 7,
        Next = 8,
        Previous = 9,
        Sit = 10,
        Exit = 11,
        Menu = 12,
        Select = 13
    }
    public static KeyCode Jump {
        set
        {
            if (isJoystickConnected)
            {
                joystickJump = value;
                return;
            }
            keyboardJump = value;
        }
        get
        {
            if (isJoystickConnected) return joystickJump;
            
            return keyboardJump;
        }
    }
    public static KeyCode Attack1
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickAttack1 = value;
                return;
            }
            keyboardAttack1 = value;
        }
        get
        {
            if (isJoystickConnected) return joystickAttack1;
            return keyboardAttack1;
        }
    }

    public static KeyCode Attack2
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickAttack2 = value;
                return;
            }
            keyboardAttack2 = value;
        }
        get
        {
            if (isJoystickConnected) return joystickAttack2;
            return keyboardAttack2;
        }
    }
    public static KeyCode Action1
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickAction1 = value;
                return;
            }
            keyboardAction1 = value;
        }
        get
        {
            if (isJoystickConnected) return joystickAction1;
            return keyboardAction1;
        }
    }

    public static KeyCode Action2
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickAction2 = value;
                return;
            }
            keyboardAction2 = value;
        }
        get
        {
            if (isJoystickConnected) return joystickAction2;
            return keyboardAction2;
        }
    }

    public static KeyCode Action3
    {
        set
        {
            if (isJoystickConnected){
                joystickAction3 = value;
                return;
            }
        keyboardAction3 = value;
        }
        get
        {
            if (isJoystickConnected) return joystickAction3;
            return keyboardAction3;
        }
    }

    public static KeyCode Action4
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickAction4 = value;
                return;
            }
            
            keyboardAction4 = value;
        }
        get
        {
            if (isJoystickConnected) return joystickAction4;
            return keyboardAction4;
        }
    }

    public static KeyCode Next
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickNext = value;
                return;
            }

            keyboardNext = value;
        }
        get
        {
            if (isJoystickConnected) return joystickNext;
            return keyboardNext;
        }
    }

    public static KeyCode Previous
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickPrevious = value;
                return;
            }

            keyboardPrevious = value;
        }
        get
        {
            if (isJoystickConnected) return joystickPrevious;
            return keyboardPrevious;
        }
    }

    public static KeyCode Sit
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickSit = value;
                return;
            }

            keyboardSit = value;
        }
        get
        {
            if (isJoystickConnected) return joystickSit;
            return keyboardSit;
        }
    }

    public static KeyCode Menu
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickMenu = value;
                return;
            }

            keyboardMenu = value;
        }
        get
        {
            if (isJoystickConnected) return joystickMenu;
            return keyboardMenu;
        }
    }

    public static KeyCode Select
    {
        set
        {
            if (isJoystickConnected)
            {
                joystickSelect = value;
                return;
            }

            keyboardSelect = value;
        }
        get
        {
            if (isJoystickConnected) return joystickSelect;
            return keyboardSelect;
        }
    }

    private static KeyCode keyboardJump = KeyCode.Space;

    private static KeyCode keyboardAttack1 = KeyCode.Mouse0;
    private static KeyCode keyboardAttack2 = KeyCode.Mouse1;

    private static KeyCode keyboardAction1 = KeyCode.E;
    private static KeyCode keyboardAction2 = KeyCode.R;
    private static KeyCode keyboardAction3 = KeyCode.F;
    private static KeyCode keyboardAction4 = KeyCode.G;

    private static KeyCode keyboardNext = KeyCode.Alpha2;
    private static KeyCode keyboardPrevious = KeyCode.Alpha1;
    private static KeyCode keyboardSit = KeyCode.C;
    private static KeyCode keyboardMenu = KeyCode.Escape;
    private static KeyCode keyboardSelect = KeyCode.Return;


    private static bool isJoystickConnected = false;
    private static int mainJoystickNumber = 1;

    private static KeyCode joystickJump;
                   
    private static KeyCode joystickAttack1;
    private static KeyCode joystickAttack2;
                   
    private static KeyCode joystickAction1;
    private static KeyCode joystickAction2;
    private static KeyCode joystickAction3;
    private static KeyCode joystickAction4;

    private static KeyCode joystickNext;
    private static KeyCode joystickPrevious;
    private static KeyCode joystickSit;
    private static KeyCode joystickMenu;
    private static KeyCode joystickSelect;



    public static void Init()
    {

    }



    public static void setJoystickActive(bool value)
    {
        isJoystickConnected = value;
    }

    public static bool isJoysticActive()
    {
        return isJoystickConnected;
    }

    public static void setMainJoystickNumber(int value)
    {
        mainJoystickNumber = value;
    }

    public static int getMainJoystickNumber()
    {
        return mainJoystickNumber;
    }

   
}
