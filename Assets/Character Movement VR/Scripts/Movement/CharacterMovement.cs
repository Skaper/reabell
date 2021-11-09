using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class CharacterMovement : MonoBehaviour
{
    public CapsuleCollider bodyCollider;
    [Range(0f, 1f)] public float stepHeightRatio = 0.25f;
    public float movementSpeed = 3f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    public LayerMask layermask = 0;

    public bool DebugDraw = false;

    public float sphereCastRadius;
    public float sphereCastLenght;
    public Vector3 sphreCastOffset = Vector3.zero;

    private Vector3 currentGroundAdjustmentVelocity;
    private float fallingSpeed;
    public Rigidbody rig;

    private InputBridge input;

    public bool moving { get; private set; } = false;

    void Start()
    {
        //rig = GetComponent<Rigidbody>();
        input = InputBridge.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 _velocity = CalculateMovementVelocity();
        if (!_velocity.Equals(Vector3.zero))
            moving = true;
        else
            moving = false;

        bool isGrounded = CheckIfGround();
        if (isGrounded)
        {
            fallingSpeed = 0f;
        }
        else
        {
            moving = true;
            fallingSpeed += gravity * Time.fixedDeltaTime;
            Debug.Log("Fall down");
        }

        SetVelocity(_velocity);
    }
    protected Vector3 CalculateMovementVelocity()
    {
        //Calculate (normalized) movement direction;
        Vector3 _velocity = CalculateMovementDirection();

        //Save movement direction for later;
        Vector3 _velocityDirection = _velocity;

        //Multiply (normalized) velocity with movement speed;
        _velocity *= movementSpeed;

        //If controller is not grounded, multiply movement velocity with 'airControl';
        //if (!(currentControllerState == ControllerState.Grounded))
        //    _velocity = _velocityDirection * movementSpeed * airControl;

        return _velocity;
    }
    private Vector3 CalculateMovementDirection()
    {
        Vector3 _velocity = Vector3.zero;

        if (cameraTransform == null)
        {
            _velocity += transform.right * input.LeftThumbstickAxis.x;
            _velocity += transform.forward * input.LeftThumbstickAxis.y;
        }
        else
        {
            //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
            //Project movement direction so movement stays parallel to the ground;
            _velocity += Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized * input.LeftThumbstickAxis.x;
            _velocity += Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized * input.LeftThumbstickAxis.y;
        }

        if (_velocity.magnitude > 1f)
            _velocity.Normalize();

        return _velocity;
    }


    private bool CheckIfGround()
    {
        currentGroundAdjustmentVelocity = Vector3.zero;

        //Calculate origin and direction of ray in world coordinates;
        Vector3 _worldOrigin = transform.TransformPoint(bodyCollider.center);//bodyCollider.transform.position + Vector3.up * (bodyCollider.radius + Physics.defaultContactOffset);//
        Vector3 _worldDirection = -transform.up;
        float _safetyDistanceFactor = 0.001f;
        float rayLeghth = bodyCollider.center.y + 0.01f; //+ sphereCastLenght;
        sphereCastRadius = Mathf.Clamp(bodyCollider.radius / 2f * 0.8f, _safetyDistanceFactor, (bodyCollider.height / 2f) * (1f - _safetyDistanceFactor));

        float _length = 0f;
        _length += (bodyCollider.height * (1f - stepHeightRatio)) * 0.5f;
        _length += bodyCollider.height * stepHeightRatio;

        float castLength = _length;

        bool hasHit = Physics.SphereCast(
            _worldOrigin,
            sphereCastRadius,//sphereCastRadius - Physics.defaultContactOffset,
            _worldDirection,
            out RaycastHit hitInfo,
            castLength - sphereCastRadius,///rayLeghth, 
            layermask,
            QueryTriggerInteraction.Ignore);
        //bodyCollider.transform.position + Vector3.up * (bodyCollider.radius + Physics.defaultContactOffset);//
        hasHit = Physics.SphereCast(bodyCollider.transform.position + bodyCollider.center +
            // here, to make the sphere start position higher by this offset
            Vector3.up * (bodyCollider.radius + Physics.defaultContactOffset),
            // and here to make the sphere radius lower by this offset
            bodyCollider.radius - Physics.defaultContactOffset,
            Vector3.down, out hitInfo, bodyCollider.height / 2f + stepHeightRatio,
            layermask, QueryTriggerInteraction.Ignore);
        Debug.Log("HIT: " + hasHit);
        //if (hasHit) Debug.Log(" GO " + hitInfo.collider.gameObject);

        currentGroundAdjustmentVelocity = transform.up * (fallingSpeed * Time.fixedDeltaTime);
        return hasHit;
    }


    public void SetVelocity(Vector3 _velocity)
    {
        rig.velocity = _velocity + currentGroundAdjustmentVelocity;
    }

    void OnDrawGizmosSelected()
    {
        if (DebugDraw)
        {
            Vector3 rayStart = transform.TransformPoint(bodyCollider.center);

            float rayLeghth = bodyCollider.center.y + 0.01f;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(rayStart, bodyCollider.radius + sphereCastRadius - Physics.defaultContactOffset);
        }
    }
}
