using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AsteroidSpawner : MonoBehaviour {
	[Tooltip("How many asteroids are spawned")]
	public int AsteroidNumber = 10;
	int prevAstNum;
	[Tooltip("How many clumps are spawned")]
	public int ClumpNumber = 3;
	int prevClumpNum;
	[Tooltip("Max spawn range")]
	public float Range = 10f;
	float prevRange;
	[Tooltip("How much does their scale vary")]
	public float ScaleVariation = 5f;

	public bool mode2D;
	float prevScaleVar;


	public List<GameObject> Asteroids = new List<GameObject>();
	public List<GameObject> AsteroidClumps = new List<GameObject>();

	void OnValidate(){
		if(AsteroidNumber!=prevAstNum || ClumpNumber != prevClumpNum || Range != prevRange || ScaleVariation != prevScaleVar){
			prevAstNum = AsteroidNumber;
			prevClumpNum = ClumpNumber;
			prevRange = Range;
			prevScaleVar = ScaleVariation;
			StartCoroutine(Spawn());
		}
	}

	void Start () {
		
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn(){
		yield return null;

		foreach (Transform child in GetComponentsInChildren<Transform>()) {
			if(child!=transform)
			DestroyImmediate(child.gameObject);
		}

		if(Asteroids.Count!=0 || AsteroidClumps.Count!=0){
		
			for (int i = 0; i < AsteroidNumber; i++) {
				GameObject asteroid = (GameObject) Instantiate (Asteroids [Random.Range (0, Asteroids.Count-1)],transform.position + Random.insideUnitSphere * Range, Quaternion.identity);
				if(mode2D){
					asteroid.transform.position = new Vector3(asteroid.transform.position.x, 0, asteroid.transform.position.z);
					asteroid.AddComponent<Rigidbody>();
					asteroid.GetComponent<Rigidbody>().useGravity = false;
					asteroid.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
					asteroid.GetComponent<Collider>().isTrigger = false;
				}
				asteroid.transform.localScale *= Random.Range(-ScaleVariation,ScaleVariation);
				asteroid.transform.LookAt (Random.insideUnitSphere*Range);
				asteroid.transform.parent = transform;
			}
			for (int i = 0; i < ClumpNumber; i++) {
				GameObject clump = (GameObject) Instantiate (AsteroidClumps [Random.Range (0, AsteroidClumps.Count-1)],transform.position + Random.insideUnitSphere * Range, Quaternion.identity);
				if(mode2D){
					clump.transform.position = new Vector3(clump.transform.position.x, 0, clump.transform.position.z);
				}
				clump.transform.localScale *= Random.Range(-ScaleVariation,ScaleVariation);
				clump.transform.LookAt (Random.insideUnitSphere*Range);
				foreach (Transform child in clump.GetComponentsInChildren<Transform>()) {
					child.parent = transform;
					if(mode2D){
						child.gameObject.AddComponent<Rigidbody>();
						child.gameObject.GetComponent<Rigidbody>().useGravity = false;
						child.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
						child.GetComponent<Collider>().isTrigger = false;

						if(mode2D){
							child.position = new Vector3(child.position.x, 0, child.position.z);
						}

					}
				}
				clump.transform.parent = transform;
				/*if(mode2D){
					clump.AddComponent<Rigidbody>();
					clump.GetComponent<Rigidbody>().useGravity = false;
					clump.GetComponent<Collider>().isTrigger = false;
				}*/
			}
		}else{
			Debug.LogError("No asteroid/clump prefabs to spawn");
		}
	}
		
	
}
