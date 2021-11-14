using System.Collections;
using System.Collections.Generic;
using BNG;
using UnityEngine;
using CMF;
[RequireComponent(typeof(Rigidbody))]
public class PlayerInput_Interactor : MonoBehaviour
{
    public enum HandSource
    {
        Left,
        Right
    }
    
    public HandSource hand;
    public Rigidbody xrRigRigidbody;
    public PlayerClimb climbScript;
    public VelocitiesFromTransform velocities;

    public Rigidbody MyRigidbody { get; private set; }
    public Vector3 XRRigTargetDelta
    {
        get
        {
            if (Climbing)
            {
                return -((MyRigidbody.position - xrRigRigidbody.position) - ClimbReference);
            }
            else
                return Vector3.zero;
        }
    }

    private GameObject HeldObject;
    private Interactable HeldObjectInteractable;
    private GameObject TargetObject;
    private bool waitingOnFirstRelease;
    private bool Climbing;
    private Vector3 ClimbReference;


    #region InputVarabels

    private float GripAxisValue_Left;
    private float GripAxisValue_Right;
    private float GripThreshold = 0.85f;
    private bool GripValue_Left;
    private bool GripValue_Right;

    private InputBridge input;

    #endregion

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        input = InputBridge.Instance;
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        if (hand == HandSource.Left)
        {
            if (GripAxisValue_Left != input.LeftGrip)
            {
                GripAxisValue_Left = input.LeftGrip;
            }
            bool newGripValueLeft = (GripAxisValue_Left > GripThreshold);

            if (GripValue_Left != newGripValueLeft)
            {
                GripValue_Left = newGripValueLeft;
                HandleGrab(GripValue_Left);
            }
        }
        else if(hand == HandSource.Right)
        {
            if (GripAxisValue_Right != input.RightGrip)
            {
                GripAxisValue_Right = input.RightGrip;
            }
            
            bool newGripValueRight = (GripAxisValue_Right > GripThreshold);
            
            if (GripValue_Right != newGripValueRight)
            {
                GripValue_Right = newGripValueRight;
                HandleGrab(GripValue_Right);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Interactable>() != null)
            TargetObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (TargetObject == other.gameObject)
            TargetObject = null;
    }

    // Entry into grabbing. value indicates state of button - true for starting a grab or false for ending a grab
    public void HandleGrab(bool value)
    {
        // Grab on press down
        if (value && HeldObject == null && TargetObject != null)
        {
            HeldObject = TargetObject;
            HeldObjectInteractable = HeldObject.GetComponent<Interactable>();

            if (climbScript != null
                && HeldObjectInteractable.type == Interactable.InteractableType.Climbable)
            {
                climbScript.AddInfluencer(this);
                Climbing = true;
                ResetReference();
            }
        }
        else if (!value && waitingOnFirstRelease)
        {
            waitingOnFirstRelease = false;
        }
        // Release on second release
        else if (!value)
        {
            ForceDrop();
        }
    }

    public void ForceDrop()
    {
        waitingOnFirstRelease = false;
        if (HeldObject != null)
        {
            var tempObject = HeldObject;

            if (HeldObjectInteractable.type == Interactable.InteractableType.Climbable)
            {
                climbScript.RemoveInfluencer(this);
                Climbing = false;
            }

            HeldObject = null;
            HeldObjectInteractable = null;
            tempObject.GetComponent<Interactable>().Drop();
        }
    }

    private void OnJointBreak(float breakForce)
    {
        ForceDrop();
    }
    
    public void ResetReference()
    {
        ClimbReference = (MyRigidbody.position - xrRigRigidbody.position);
    }
}
