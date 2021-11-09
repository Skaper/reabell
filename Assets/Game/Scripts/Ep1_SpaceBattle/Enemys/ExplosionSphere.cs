using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplosionSphere : MonoBehaviour
{
    public float explosionDamage = 25f;

    private float targetRadius;
    private float currentRadius;
    private SphereCollider collider;

    private bool isActive = true;


    private void Start()
    {
        collider = GetComponent<SphereCollider>();
        targetRadius = collider.radius;
        collider.radius = 0f;
        currentRadius = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActive && other.gameObject.CompareTag("Player"))
        {
            print("BOOM!");
            isActive = false;
        }
    }

}
