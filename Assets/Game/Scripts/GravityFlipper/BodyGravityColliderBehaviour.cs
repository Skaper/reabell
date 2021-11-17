using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGravityColliderBehaviour : MonoBehaviour
{
    public CapsuleCollider BodyCollider;
    private SphereCollider _gravityCollider;
    void Start()
    {
        _gravityCollider = GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        _gravityCollider.center = 
            new Vector3(0, -(BodyCollider.height) + _gravityCollider.radius + BodyCollider.radius, 0);
    }
}
