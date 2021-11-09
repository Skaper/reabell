using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;
public class BulletTurret : MonoBehaviour
{
    public float BulletLifetime = 2f;


    private Rigidbody rigidbody;

    private Vector3 hitPoint;
    private bool reflectBullet;
    private Vector3 rbForce;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(Lifetime());
    }

    // Update is called once per frame
    void Update()
    {
        if (reflectBullet && (transform.position - hitPoint).sqrMagnitude < 0.25f)
        {
            Quaternion rotation = Quaternion.Euler(
                Random.Range(-40f, 40f),
                Random.Range(-40f, 40f),
                Random.Range(-40f, 40f));
            Vector3 newForce = rotation * rbForce;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(-0.5f * newForce);
        }
    }

    

    

    

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(BulletLifetime);

        Destroy(gameObject);
    }

    public void AddForce(Vector3 force)
    {
        rbForce = force;
        rigidbody.AddForce(force);
    }

    public void setHitPoint(Vector3 point)
    {
        reflectBullet = true;
        hitPoint = point;
    }
}
