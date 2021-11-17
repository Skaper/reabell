
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

    public Transform MainCamera;
    //float rotationAmountX = 0;
    private Vector2 _rotationAmount;
    private Vector2 _previousInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var axis = GetAxisInput();
        axis = new Vector2(axis.x, -axis.y);
        DoSnapRotation(axis.x, _previousInput.x, system.xrRig.transform.up);
        DoSnapRotation(axis.y, _previousInput.y, MainCamera.right);//system.xrRig.transform.right
        _previousInput = axis;
    }

    public virtual void DoSnapRotation(float xInput, float previousInput, Vector3 vector)
    {

        // Reset rotation amount before retrieving inputs
        var rotationAmount = 0f;

        // Snap Right
        if (xInput >= SnapInputAmount && previousInput < SnapInputAmount)
        {
            rotationAmount += SnapRotationAmount;
        }
        // Snap Left
        else if (xInput <= -SnapInputAmount && previousInput > -SnapInputAmount)
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
            BeginLocomotion();
            //system.xrRig.RotateAroundCameraUsingRigUp(angle.x);
            system.xrRig.RotateAroundCameraPosition(vector, rotationAmount);
            EndLocomotion();
            //Turn(rotationAmountX);
        }
    }

    private void Turn(Vector2 angle)
    {
        BeginLocomotion();
        system.xrRig.RotateAroundCameraUsingRigUp(angle.x);
        system.xrRig.RotateAroundCameraPosition(system.xrRig.transform.right, angle.y);
        EndLocomotion();
    }

    private Vector2 GetAxisInput()
    {

        // Use the largest, non-zero value we find in our input list
        Vector2 lastVal = Vector2.zero;

        for (int i = 0; i < inputAxis.Count; i++)
        {
            var axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]);

            // Always take this value if our last entry was 0. 
            if (lastVal == Vector2.zero)
            {
                lastVal = axisVal;
            }
            else if (axisVal != Vector2.zero && axisVal.magnitude > lastVal.magnitude)
            {
                lastVal = axisVal;
            }
        }

        return lastVal;
    }
}

