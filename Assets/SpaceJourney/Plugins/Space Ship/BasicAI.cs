using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour {

	Transform player;
	
	[Tooltip("The point around which the ship flies")]
	public Transform origin;
	[Tooltip("The bullet prefab")]
	public GameObject bullet;
	[Tooltip("Where do the bullets come from?")]
	public Transform barrel;
	[Tooltip("Range of flying around origin")]
	public float range = 1000f;
	[Tooltip("The distance from player after which the ship changes course")]
	public float diveDis = 150f;
	[Tooltip("Is the ship hunting down the player?")]
	public bool aggresive = false;
	[Tooltip("How long the ship will try to escape the player")]
	public float timeInThreatenedMode = 20f;
	[Tooltip("How fast does the ship go when in aggresive mode")]
	public float aggresiveSpeed = 2.75f;
	[Tooltip("How fast does the ship turn when in aggresive mode (the smaller the value the faster the turn)")]
	public float aggresiveTurnSpeed = 0.65f;
	[Tooltip("How fast does the ship go")]
	public float normalSpeed = 1f;
	[Tooltip("How fast does the ship turn (the smaller the value the faster the turn)")]
	public float normalTurnSpeed = 10f;
	[Tooltip("2D mode for AI movement")]
	public bool mode2D;
	float prevspeed, prevturn;


	List<Vector3> points = new List<Vector3>();

	Rigidbody rigid; 
	[HideInInspector]
	public Coroutine shoot;

	WaitForFixedUpdate wait = new WaitForFixedUpdate();

	IEnumerator firing(){

		GameObject bulletObj;
		while(true){
		
			bulletObj = (GameObject)Instantiate (bullet,barrel.position, Quaternion.LookRotation(transform.forward,transform.up));
			if (bulletObj.GetComponent<BulletScript> () != null) {
				bulletObj.GetComponent<BulletScript> ().ship = transform.root;
			}
			bulletObj.GetComponent<Rigidbody> ().AddForce (bulletObj.transform.forward*450f,ForceMode.Impulse);
			yield return new WaitForSeconds(0.2f);
			
		}

	}

	void Start () {

		if(!mode2D){
			player = SpaceshipController.instance.transform;
		}else{
			player = SpaceshipController2D.instance.transform;
		}



		Vector3 point;

		for (int i = 0; i < 50; i++) {
			point = origin.position + Random.insideUnitSphere * range;
			if(mode2D){
				point.y = 0;
			}
			points.Add (point);
		}
		
		rigid = GetComponent<Rigidbody> ();
		if (!aggresive) {
			StartCoroutine (going ());
		} else {
			StartCoroutine (chasing ());
		}
	}


	IEnumerator going(){

		int j = 0;

		while (true) {

			while (Vector3.Distance (transform.position, points [j]) > 7f) {
				transform.Translate (transform.forward*normalSpeed, Space.World);
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (points [j] - transform.position, transform.up), Time.deltaTime/normalTurnSpeed);
				yield return wait;
			}

			j++;
			if (j > points.Count - 1) {
				j = 0;
			}
		
		}
	}

	public void threat(){
		if (!aggresive) {
			StartCoroutine (threatened());
		}
	}

	public IEnumerator threatened(){

		prevturn = normalTurnSpeed;
		prevspeed = normalSpeed;
		normalSpeed = aggresiveSpeed;
		normalTurnSpeed = aggresiveTurnSpeed;

		yield return new WaitForSeconds(timeInThreatenedMode);

		normalSpeed = prevspeed;
		normalTurnSpeed = prevturn;

	}
	[HideInInspector]
	public bool shooting = false;

	public IEnumerator chasing(){

		bool diving = true;

		

		//Vector3 rand_offset = new Vector3 (Random.Range(-20f,100f),Random.Range(-100f,100f),Random.Range(-100f,100f));

		while (player.gameObject.activeSelf) {

			/*Vector3 dirFromAtoB = (player.position - transform.position).normalized;
			float dotProd = Vector3.Dot(dirFromAtoB, transform.forward);
			
			if(dotProd > 0.85f) {
				shoot = StartCoroutine(firing());
			}else{
				if(shoot!=null){
					StopCoroutine(shoot);
				}

			}*/

			if(diving){
				while(Vector3.Distance( transform.position, player.position)> diveDis){

					if(!shooting){
						shoot = StartCoroutine(firing());
						shooting = true;
					}

					transform.Translate (transform.forward*aggresiveSpeed, Space.World);
					transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (player.position - transform.position, transform.up), Time.deltaTime/aggresiveTurnSpeed);

					yield return wait;

				}
				diving = false;
				
			}else{
				
				int k = Random.Range(0,points.Count-1);

				while(Vector3.Distance( transform.position, points[k])> 50f){

					if(shooting){
						StopCoroutine(shoot);
						shooting = false;
					}

					transform.Translate (transform.forward*aggresiveSpeed, Space.World);
					transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (points[k] - transform.position, transform.up), Time.deltaTime/aggresiveTurnSpeed);

					yield return wait;

				}
				diving = true;

			}

			yield return wait;

		}

		if(shoot!=null){
			StopCoroutine(shoot);
		}
		StartCoroutine(going());

	}
}
