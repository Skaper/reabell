using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;
public class AITurret : MonoBehaviour
{

    
    
    public GameObject bulletPrefub;

    public Transform muzzlePoint;
    public Transform weaponRay;

    public GameObject HitEffect;

    public float shootDistance = 100f;
    public float shootDelay = 1.5f;

    public float turnSpeed = 1f;

    public float bulletForce = 200f;

    private float shootTimer;

    public bool canShoot = true;
    public float bulletDamage = 20f;

    private Transform target;
    private float targetHeight;
    private float targetDiameter;

    void Start()
    {
        CapsuleCollider targetCollider = GameManager.PlayerReferences.BodyCollider;
        targetHeight = targetCollider.height;
        targetDiameter = targetCollider.radius * 2f;
        target = targetCollider.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null && canShoot)
        {
            Vector3 newTargetPos = new Vector3(
                target.position.x + Random.Range(-targetDiameter, targetDiameter),
                target.position.y + Random.Range(-targetHeight, targetHeight),
                target.position.z);
            Vector3 direction = (newTargetPos - transform.position).normalized;

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
            Physics.Raycast(
                weaponRay.position,
                weaponRay.TransformDirection(Vector3.forward),
                out weaponHit,
                shootDistance);

            if (weaponHit.collider)
            {
                
                if (shootTimer >= shootDelay)
                {

                    string hitTag = weaponHit.collider.gameObject.tag;
                    if (hitTag.Equals("Player") ||
                        hitTag.Equals("Player/Shield"))
                    {
                        BulletTurret bullet = Instantiate(bulletPrefub, muzzlePoint.position, transform.rotation).GetComponent<BulletTurret>();

                        bullet.AddForce(direction * bulletForce);
                        
                        if (hitTag.Equals("Player/Shield"))
                        {
                            bullet.setHitPoint(weaponHit.point);
                        }
                        if (hitTag.Equals("Player"))
                        {
                            

                            PlayerHealthController ph = GameManager.PlayerReferences.PlayerHealthController;

                            if (ph != null)
                            {
                                ph.TakeDamage(bulletDamage);
                            }
                        }
                        

                        GameObject explosion = null;
                        explosion = Instantiate(HitEffect, weaponHit.point, Quaternion.identity);
                        explosion.GetComponent<ParticleSystem>().Play();
                        Destroy(explosion, 5f);
                        shootTimer = 0f;

                    }
                    
                }
                    
            }
            Debug.DrawRay(weaponRay.position, weaponRay.TransformDirection(Vector3.forward) * 1000, Color.red);

            shootTimer += Time.deltaTime;
        }
    }

}
