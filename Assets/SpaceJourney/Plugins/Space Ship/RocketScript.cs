using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour {

	[Tooltip("The explosion particle system")]
	public ParticleSystem Explosion;

	public GameObject followParticle;

	void Start(){
		StartCoroutine (Lifetime());
	}

	void OnTriggerEnter(Collider other){
		if (Vector3.Distance (other.transform.position, transform.position) > 15f) {
			if (other.GetComponent<DestructionScript> () != null) {
				if(SpaceshipController.instance != null){
					other.GetComponent<DestructionScript> ().HP -= SpaceshipController.instance.m_shooting.rocketSettings.RocketDamage;
				}else{
					other.GetComponent<DestructionScript> ().HP -= SpaceshipController2D.instance.m_shooting.rocketSettings.RocketDamage;
				}
			}
			if (other.GetComponent<BasicAI> () != null) {
				other.GetComponent<BasicAI> ().threat ();
			}
			StartCoroutine (Boom (other.transform));
		}
	}

	public void StartChase(Transform target, float rocketSpeed, float RocketInitialSpeed)
    {
		StartCoroutine (Chase (target, rocketSpeed, RocketInitialSpeed));
	}

	public IEnumerator Chase(Transform target, float rocketSpeed, float RocketInitialSpeed)
    {
		Vector3 direction;
        Vector3 initialdirection= target.position - transform.position;
        //transform.LookAt();
        Quaternion lookRotation;

        //Debug.Log("Initial pos " + target.position);

        //Vector3 pos = transform.position;
        //Vector3 p1 = ((transform.position + target.position) / 2f) + new Vector3(Random.Range(-10f,10f),Random.Range(-10f,10f),Random.Range(-10f,10f)) ;


        //Old script

        /*
		while (true) {
		
			direction = target.position - transform.position;
			//Debug.Log(direction.normalized);
			lookRotation = Quaternion.LookRotation(direction.normalized);
			transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 7f);
			//transform.LookAt(target);
			transform.Translate(transform.forward*rocketSpeed,Space.World);
			if(Vector3.Distance(transform.position, target.position) < 10f){

				if (target.GetComponent<DestructionScript> () != null) {
					if(SpaceshipController.instance!=null){
						target.GetComponent<DestructionScript> ().HP -= SpaceshipController.instance.m_shooting.rocketSettings.RocketDamage;
					}else{
						target.GetComponent<DestructionScript> ().HP -= SpaceshipController2D.instance.m_shooting.rocketSettings.RocketDamage;
					}
					//Debug.Log (target.GetComponent<DestructionScript> ().HP);
				}
				if (target.GetComponent<BasicAI> () != null) {
					target.GetComponent<BasicAI> ().threat ();
				}
				StartCoroutine (Boom (target));
			//Debug.Log (target.name);

			}
            
			//GetComponent<Rigidbody>().AddForce(direction.normalized * rocketSpeed, ForceMode.Acceleration);
			yield return null;
		}*/
        // End of old script
        GetComponent<Rigidbody>().AddForce(initialdirection.normalized * RocketInitialSpeed, ForceMode.Impulse);
        transform.rotation = Quaternion.LookRotation(initialdirection.normalized);
        //Debug.Log(initialdirection);
        while (true)
        {
            direction = target.position - transform.position;

            
            lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 100f);
            GetComponent<Rigidbody>().AddForce(transform.forward * rocketSpeed, ForceMode.Acceleration);
            //transform.Translate(direction.normalized * rocketSpeed/100, Space.World);
            //transform.Translate(transform.forward*rocketSpeed,Space.World);

            if (Vector3.Distance(transform.position, target.position) < 10f)
            {

                if (target.GetComponent<DestructionScript>() != null)
                {
                    if (SpaceshipController.instance != null)
                    {
                        target.GetComponent<DestructionScript>().HP -= SpaceshipController.instance.m_shooting.rocketSettings.RocketDamage;
                    }
                    else
                    {
                        target.GetComponent<DestructionScript>().HP -= SpaceshipController2D.instance.m_shooting.rocketSettings.RocketDamage;
                    }
                    //Debug.Log (target.GetComponent<DestructionScript> ().HP);
                }
                if (target.GetComponent<BasicAI>() != null)
                {
                    target.GetComponent<BasicAI>().threat();
                }
                StartCoroutine(Boom(target));
                //Debug.Log (target.name);

            }
            yield return null;
        }
    }

	IEnumerator Lifetime(){
		if(SpaceshipController.instance!=null){
			yield return new WaitForSeconds (SpaceshipController.instance.m_shooting.rocketSettings.RocketLifetime);
		}else{
			yield return new WaitForSeconds (SpaceshipController2D.instance.m_shooting.rocketSettings.RocketLifetime);
		}
		Destroy (gameObject);
	}

	public IEnumerator Boom(Transform par){
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		if(followParticle!=null){
			followParticle.SetActive(false);
		}
		transform.parent = par;
		GetComponent<Renderer> ().enabled = false;
		if (Explosion != null) {
            //Explosion.Play ();
            //Debug.Log("test");
            GameObject ExplosionEffect = transform.GetChild(1).gameObject;
            ExplosionEffect.SetActive(true);
            //xplosionEffect.Play();
		}
		yield return new WaitForSeconds (2f);
		Destroy (gameObject);
	}
}
