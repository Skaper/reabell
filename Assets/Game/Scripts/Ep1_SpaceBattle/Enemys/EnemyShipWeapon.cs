using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipWeapon : MonoBehaviour
{
    public GameObject bulletPrefub;
    public string targetTag = "Player";
    public Transform muzzlePoint;
    public Transform weaponRay;

    public float shootDistance = 100f;
    public float shootDelay = 1.5f;

    public float turnSpeed = 1f;

    public float bulletForce = 200f;

    private GameObject target;

    private int layerMask = 1 << 8;
    private float shootTimer;

    public bool canShoot = true;
    public float bulletDamage = 20f;

    void Start()
    {
        layerMask = ~layerMask;
        OnStart();
    }
    protected virtual void OnStart()
    {

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null && canShoot)
        {

            Vector3 direction = (target.transform.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            if (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
            {
                transform.rotation = Quaternion.Lerp(
                            transform.rotation,
                            targetRotation,
                            Time.deltaTime * turnSpeed
                        );
            }

            RaycastHit weaponHit;
            DoRayCast(out weaponHit);

            if (weaponHit.collider)
            {
                if (weaponHit.collider.gameObject.CompareTag(targetTag) && shootTimer >= shootDelay)
                {
                    DoShoot(direction, weaponHit);
                    shootTimer = 0f;
                }
            }
            Debug.DrawRay(weaponRay.position, weaponRay.TransformDirection(Vector3.forward) * 1000, Color.red);

            shootTimer += Time.deltaTime;
        }   
    }

    protected virtual void DoRayCast(out RaycastHit weaponHit)
    {
        Physics.Raycast(
                   weaponRay.position,
                   weaponRay.TransformDirection(Vector3.forward),
                   out weaponHit,
                   shootDistance,
                   layerMask,
                   QueryTriggerInteraction.Ignore);
    }

    protected virtual void DoShoot(Vector3 direction, RaycastHit weaponHit)
    {
        Debug.Log("Shoot");
        SimpleBullet bullet = Instantiate(bulletPrefub, muzzlePoint.position, Quaternion.identity).GetComponent<SimpleBullet>();
        bullet.damage = bulletDamage;
        bullet.creatorTag = tag;
        bullet.AddForce(direction * bulletForce);
        
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
