using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.XR;

public class PlayerInput : MonoBehaviour
{
    private float GripAxisValue_Left;
    private float GripAxisValue_Right;
    private bool TriggerValue_Left;
    private bool TriggerValue_Right;
    private bool GripValue_Left;
    private bool GripValue_Right;
    private bool MenuValue_Left;
    private bool MenuValue_Right;
    private bool PrimaryAxis2DTouch_Left;
    private bool PrimaryAxis2DTouch_Right;
    private bool PrimaryAxis2DClick_Left;
    private bool PrimaryAxis2DClick_Right;
    private float TriggerAxisValue_Left;
    private float TriggerAxisValue_Right;
    private Vector2 PrimaryAxis2DValue_Left;
    private Vector2 PrimaryAxis2DValue_Right;

    private float TriggerThreshold = 0.9f;
    private float GripThreshold = 0.85f;

    public delegate void BoolEventHandler(bool value);
    public delegate void FloatEventHandler(float value);
    public delegate void Vector2EventHandler(Vector2 value);

    public event FloatEventHandler GripAxisInputEvent_Left;
    public event FloatEventHandler GripAxisInputEvent_Right;

    public event BoolEventHandler GripInputEvent_Left;
    public event BoolEventHandler GripInputEvent_Right;

    public event BoolEventHandler TriggerInputEvent_Left;
    public event BoolEventHandler TriggerInputEvent_Right;

    public event BoolEventHandler MenuInputEvent_Left;
    public event BoolEventHandler MenuInputEvent_Right;

    public event BoolEventHandler Primary2DAxisTouchInputEvent_Left;
    public event BoolEventHandler Primary2DAxisTouchInputEvent_Right;
    public event BoolEventHandler Primary2DAxisClickInputEvent_Left;
    public event BoolEventHandler Primary2DAxisClickInputEvent_Right;

    public event FloatEventHandler TriggerAxisInputEvent_Left;
    public event FloatEventHandler TriggerAxisInputEvent_Right;
    public event Vector2EventHandler Primary2DAxisInputEvent_Left;
    public event Vector2EventHandler Primary2DAxisInputEvent_Right;

    private InputBridge input;


    public enum HandSource
    {
        Left,
        Right
    }

    public enum Actions
    {
        Grip,
        Trigger,
        Primary2DAxis,
        Menu
    }

    private void Start()
    {
        input = InputBridge.Instance;
        UpdateInputs();
    }

    private void Update()
    {
        UpdateInputs();
    }

    public bool GetBoolValue(Actions action, HandSource hand)
    {
        if (hand == HandSource.Left)
        {
            switch (action)
            {
                case Actions.Grip:
                    return GripAxisValue_Left > 0 ? true : false;
                case Actions.Trigger:
                    return TriggerValue_Left;
                case Actions.Menu:
                    return MenuValue_Left;
            }
        }
        else
        {
            switch (action)
            {
                case Actions.Grip:
                    return GripAxisValue_Right > 0 ? true : false;
                case Actions.Trigger:
                    return TriggerValue_Right;
                case Actions.Menu:
                    return MenuValue_Right;
            }
        }

        Debug.LogError("No bool action \"" + action + "\" found in PlayerInput");
        return false;
    }

    public Vector2 GetVector2Value(Actions action, HandSource hand)
    {
        if (hand == HandSource.Left)
        {
            switch (action)
            {
                case Actions.Primary2DAxis:
                    return PrimaryAxis2DValue_Left;
            }
        }
        else
        {
            switch (action)
            {
                case Actions.Primary2DAxis:
                    return PrimaryAxis2DValue_Right;
            }
        }

        Debug.LogError("No Vector2 action \"" + action + "\" found in PlayerInput");
        return new Vector2();
    }

    private void UpdateInputs()
    {
        if (GripAxisValue_Left != input.LeftGrip)
        {
            GripAxisValue_Left = input.LeftGrip;
            GripAxisInputEvent_Left?.Invoke(GripAxisValue_Left);
        }

        if (GripAxisValue_Right != input.RightGrip)
        {
            GripAxisValue_Right = input.RightGrip;
            GripAxisInputEvent_Right?.Invoke(GripAxisValue_Right);
        }

        if (TriggerAxisValue_Left != input.LeftTrigger)
        {
            TriggerAxisValue_Left = input.LeftTrigger;
            TriggerAxisInputEvent_Left?.Invoke(TriggerAxisValue_Left);
        }

        if (TriggerAxisValue_Right != input.RightTrigger)
        {
            TriggerAxisValue_Right = input.RightTrigger;
            TriggerAxisInputEvent_Right?.Invoke(TriggerAxisValue_Right);
        }

        bool newTriggerValue_Left = (TriggerAxisValue_Left > TriggerThreshold);
        bool newTriggerValue_Right = (TriggerAxisValue_Right > TriggerThreshold);

        if (TriggerValue_Left != newTriggerValue_Left)
        {
            TriggerValue_Left = newTriggerValue_Left;
            TriggerInputEvent_Left?.Invoke(TriggerValue_Left);
        }

        if (TriggerValue_Right != newTriggerValue_Right)
        {
            TriggerValue_Right = newTriggerValue_Right;
            TriggerInputEvent_Right?.Invoke(TriggerValue_Right);
        }

        bool newGripValueLeft = (GripAxisValue_Left > GripThreshold);
        bool newGripValueRight = (GripAxisValue_Right > GripThreshold);

        if (GripValue_Left != newGripValueLeft)
        {
            GripValue_Left = newGripValueLeft;
            GripInputEvent_Left?.Invoke(GripValue_Left);
        }
        if (GripValue_Right != newGripValueRight)
        {
            GripValue_Right = newGripValueRight;
            GripInputEvent_Right?.Invoke(GripValue_Right);
        }


        if (MenuValue_Left != input.BackButton)
        {
            MenuValue_Left = input.BackButton;
            MenuInputEvent_Left?.Invoke(MenuValue_Left);
        }

        if (MenuValue_Right != input.StartButton)
        {
            MenuValue_Right = input.StartButton;
            MenuInputEvent_Right?.Invoke(MenuValue_Right);
        }

        //if (PrimaryAxis2DTouch_Left != Input.GetButton("XR_Left_Primary2DAxisTouch"))
        //{
        //    PrimaryAxis2DTouch_Left = Input.GetButton("XR_Left_Primary2DAxisTouch");
        //    Primary2DAxisTouchInputEvent_Left?.Invoke(PrimaryAxis2DTouch_Left);
        //}
        //
        //if (PrimaryAxis2DTouch_Right != Input.GetButton("XR_Right_Primary2DAxisTouch"))
        //{
        //    PrimaryAxis2DTouch_Right = Input.GetButton("XR_Right_Primary2DAxisTouch");
        //    Primary2DAxisTouchInputEvent_Right?.Invoke(PrimaryAxis2DTouch_Right);
        //}
        //
        //if (PrimaryAxis2DClick_Left != Input.GetButton("XR_Left_Primary2DAxisClick"))
        //{
        //    PrimaryAxis2DClick_Left = Input.GetButton("XR_Left_Primary2DAxisClick");
        //    Primary2DAxisClickInputEvent_Left?.Invoke(PrimaryAxis2DClick_Left);
        //}
        //
        //if (PrimaryAxis2DClick_Right != Input.GetButton("XR_Right_Primary2DAxisClick"))
        //{
        //    PrimaryAxis2DClick_Right = Input.GetButton("XR_Right_Primary2DAxisClick");
        //    Primary2DAxisClickInputEvent_Right?.Invoke(PrimaryAxis2DClick_Right);
        //}

        if (PrimaryAxis2DValue_Left.x != input.LeftThumbstickAxis.x ||
            PrimaryAxis2DValue_Left.y != input.LeftThumbstickAxis.y)
        {
            PrimaryAxis2DValue_Left.x = input.LeftThumbstickAxis.x;
            PrimaryAxis2DValue_Left.y = input.LeftThumbstickAxis.y;
            Primary2DAxisInputEvent_Left?.Invoke(PrimaryAxis2DValue_Left);
        }

        if (PrimaryAxis2DValue_Right.x != input.RightThumbstickAxis.x ||
            PrimaryAxis2DValue_Right.y != input.RightThumbstickAxis.y)
        {
            PrimaryAxis2DValue_Right.x = input.RightThumbstickAxis.x;
            PrimaryAxis2DValue_Right.y = input.RightThumbstickAxis.y;
            Primary2DAxisInputEvent_Right?.Invoke(PrimaryAxis2DValue_Right);

        }
    }
}
