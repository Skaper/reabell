using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBullet : MonoBehaviour
{
    [Tooltip("The particle effect of hitting something.")]
    public ParticleSystem bullet;
    public GameObject HitEffectAsteroid;
    public GameObject HitEffectShield;
    public float BulletLifetime = 5f;
    public float damage = 25f;

    public string creatorTag;

    private Rigidbody rigidbody;
    

    private float startTime;
    public float triggerDisableTime = 0.25f;

    private enum HitType
    {
        asteroid,
        shield
    }

    private void Awake()
    {
        startTime = Time.time;
        rigidbody = GetComponent<Rigidbody>();

    }

    void Start()
    {
        
        StartCoroutine(Lifetime());
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - startTime  < triggerDisableTime) return;
        if (other.isTrigger) return;
        if (other.CompareTag(creatorTag)) return;
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Hit(HitType.shield);
            //StartCoroutine(DestroySequence(other.gameObject, HitType.shield));
        }
        else if (!other.CompareTag("Player"))
        {
            Hit(HitType.asteroid);
            //StartCoroutine(DestroySequence(other.gameObject, HitType.asteroid));
        }
    }

    private void Hit(HitType type)
    {
        GameObject explosion = null;
        switch (type)
        {
            case HitType.shield:
                explosion = Instantiate(HitEffectShield, transform.position, Quaternion.identity);
                explosion.GetComponent<ParticleSystem>().Play();
                break;
            case HitType.asteroid:
                explosion = Instantiate(HitEffectAsteroid, transform.position, Quaternion.identity);
                explosion.GetComponent<ParticleSystem>().Play();
                break;
        }
        Destroy(explosion, 5f);
        Destroy(gameObject);
    }

  

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(BulletLifetime);
        
        Destroy(gameObject);
    }

    public void AddForce(Vector3 force)
    {
        rigidbody.AddForce(force);
    }


}
