using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class CharacterControllerMovement : MonoBehaviour
{
    public LayerMask layermask = 0;
    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    public float speed = 5f;
    public Transform cameraTransform;


    private CharacterController character;
    private Vector3 moveVector;

    private InputBridge input;

    public bool moving { get; private set; } = false;
    void Start()
    {
        character = GetComponent<CharacterController>();
        input = InputBridge.Instance;
    }

    private void FixedUpdate()
    {
        moveVector = Vector3.zero;

        //Quaternion headYaw = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);
        Vector3 direction = Vector3.zero;
        direction += Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized * input.LeftThumbstickAxis.x;
        direction += Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized * input.LeftThumbstickAxis.y;
        if (direction.magnitude > 1f)
            direction.Normalize();
        //Vector3 direction = headYaw * new Vector3(input.LeftThumbstickAxis.x, 0f, input.LeftThumbstickAxis.y);

        character.Move(direction * Time.fixedDeltaTime * speed);

        bool isGrounded = CheckIfGrounded();
        if (!isGrounded)
        {
            moving = true;
            moveVector += gravity;
            Debug.Log("Fall down");
        }

        character.Move(moveVector * Time.fixedDeltaTime);
    }

    bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(
            rayStart,
            character.radius,
            -transform.up,
            out RaycastHit hitInfo,
            rayLength,
            layermask,
            QueryTriggerInteraction.Ignore);

        return hasHit;
    }
}
