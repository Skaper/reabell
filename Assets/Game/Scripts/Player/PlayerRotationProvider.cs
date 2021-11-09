
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using BNG;
public class PlayerRotationProvider : LocomotionProvider
{


    [Tooltip("Used to determine whether to turn left / right. This can be an X Axis on the thumbstick, for example. -1 to snap left, 1 to snap right.")]
    public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.RightThumbStickAxis };

    [Tooltip("Thumbstick X axis must be >= this amount to be considered an input event")]
    public float SnapInputAmount = 0.75f;

    [Tooltip("How m any degrees to rotate if RotationType is set to 'Snap'")]
    public float SnapRotationAmount = 45f;

    [Tooltip("Allow Q,E to rotate player")]
    public bool AllowKeyboardInputs = true;

    public Transform floorOffset;
    float rotationAmount = 0;

    float previousXInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = GetAxisInput();
        DoSnapRotation(xAxis);
        previousXInput = xAxis;
    }

    public virtual void DoSnapRotation(float xInput)
    {

        // Reset rotation amount before retrieving inputs
        rotationAmount = 0;

        // Snap Right
        if (xInput >= SnapInputAmount && previousXInput < SnapInputAmount)
        {
            rotationAmount += SnapRotationAmount;
        }
        // Snap Left
        else if (xInput <= -SnapInputAmount && previousXInput > -SnapInputAmount)
        {
            rotationAmount -= SnapRotationAmount;
        }

        // Only allow keyboard if no vr input provided
        if (AllowKeyboardInputs && rotationAmount == 0)
        {
            //Use keys to ratchet rotation
            if (Input.GetKeyDown(KeyCode.Q))
            {
                rotationAmount -= SnapRotationAmount;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                rotationAmount += SnapRotationAmount;
            }
        }

        if (Mathf.Abs(rotationAmount) > 0)
        {
            //Rotate(rotationAmount);
            Turn(rotationAmount);
        }
    }

    private void Turn(float angle)
    {
        BeginLocomotion();
        system.xrRig.RotateAroundCameraUsingRigUp(angle);
        EndLocomotion();
    }

    public virtual float GetAxisInput()
    {

        // Use the largest, non-zero value we find in our input list
        float lastVal = 0;

        for (int i = 0; i < inputAxis.Count; i++)
        {
            float axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]).x;

            // Always take this value if our last entry was 0. 
            if (lastVal == 0)
            {
                lastVal = axisVal;
            }
            else if (axisVal != 0 && axisVal > lastVal)
            {
                lastVal = axisVal;
            }
        }

        return lastVal;
    }
}

