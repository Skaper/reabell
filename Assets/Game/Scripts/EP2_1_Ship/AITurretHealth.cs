using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;
public class AITurretHealth : vHealthController
{
    [vEditorToolbar("Settings", order = 200)]
    public GameObject explosion;
    public Rigidbody weaponRigidBody;
    public AITurret aiTurret;

    private bool isDestroy = false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0 && !isDestroy)
        {
            aiTurret.enabled = false;
            explosion.SetActive(true);
            isDestroy = true;
            weaponRigidBody.isKinematic = false;
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 2f);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(1500f, explosionPos, 2f, 3.0F);
            }
        }
    }
}
