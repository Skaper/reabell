using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipWeaponEmulation : EnemyShipWeapon
{
    public GameObject explosionEffect;
    private Transform player;
    
    protected override void OnStart()
    {
        player = GameManager.Player.transform;
    }

    protected override void DoShoot(Vector3 direction, RaycastHit weaponHit)
    {
        GameObject go = Instantiate(explosionEffect, weaponHit.point, Quaternion.identity);
        float distToPlayer = Vector3.Distance(weaponHit.point, player.position);
        Debug.Log("Distance to player" + distToPlayer);
        if (distToPlayer <= 10f)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(10f-distToPlayer, 4f, 1f, 1f);
        }
        Destroy(go, 5f);
    }
}
