using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour {

	public float spaceshipPushDistance = 125f;
	public float projectilePushDistance = 35f;
	public float pushSpeedMod = 1.05f;
	bool running = false;

	public bool physics;
	void OnTriggerEnter(Collider other)
	{
        try
        {
            if ((!running || other.transform.parent == SpaceshipController.instance.transform) && !physics)
            {
                StartCoroutine(Moving(other.transform));
                
            }
        }
        catch { }
	}

	IEnumerator Moving(Transform other){
		running = true;
		Vector3 dir = (transform.position - other.position).normalized;
		Vector3 dest=new Vector3();

        try
        {
           
            if (other.transform.parent == SpaceshipController.instance.transform)
            {
               
                dest = transform.position + ((dir) * spaceshipPushDistance);
            }
            else
            {
                
                dest = transform.position + ((dir) * projectilePushDistance);
            }
        }
        catch { dest = transform.position + ((dir) * projectilePushDistance); }
        while (Vector3.Distance(transform.position, dest) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, dest, Time.deltaTime * pushSpeedMod);
                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(-dir, transform.up),
                        Time.deltaTime / 3.5f);
                yield return null;
            }
        
        
		running = false;
	}

}
