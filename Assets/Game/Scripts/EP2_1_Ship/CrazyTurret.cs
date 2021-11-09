using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector;
public class CrazyTurret : AITurret
{
    

    private float shootTimer;


    private float _shootDelay;
    private float _turnSpeed;

    void Start()
    {
        _shootDelay = shootDelay;
        _turnSpeed = turnSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canShoot)
        {
            
            Vector3 direction = (weaponRay.position - transform.position).normalized;

            transform.RotateAround(transform.position, transform.up, Time.deltaTime * turnSpeed);

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

                    BulletTurret bullet = Instantiate(bulletPrefub, muzzlePoint.position, transform.rotation).GetComponent<BulletTurret>();
                    bullet.AddForce(direction * bulletForce);

                    bullet.setHitPoint(weaponHit.point);

                    if (hitTag.Equals("Player") ||
                        hitTag.Equals("Player/Shield"))
                    {
                        if (hitTag.Equals("Player"))
                        {
                            PlayerHealthController ph = GameManager.PlayerReferences.playerHealthController;

                            if (ph != null)
                            {
                                ph.TakeDamage(bulletDamage);
                            }
                        }


                        

                    }
                    GameObject explosion = null;
                    explosion = Instantiate(HitEffect, weaponHit.point, Quaternion.identity);
                    explosion.GetComponent<ParticleSystem>().Play();
                    Destroy(explosion, 5f);
                    shootTimer = 0f;
                    shootDelay = Random.Range(_shootDelay / 3f, _shootDelay * 1.5f);
                    turnSpeed = Random.Range(_turnSpeed / 2, _turnSpeed * 3f);
                }

            }
            Debug.DrawRay(weaponRay.position, weaponRay.TransformDirection(Vector3.forward) * 1000, Color.red);

            shootTimer += Time.deltaTime;
        }
    }

}
