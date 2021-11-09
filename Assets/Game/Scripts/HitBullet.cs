using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBullet : MonoBehaviour
{
    public GameObject hitEffectObject;
    public void onHit(RaycastHit hitInfo)
    {
        if(!hitInfo.collider.isTrigger) Instantiate(hitEffectObject, hitInfo.point, transform.rotation);
    }
}
