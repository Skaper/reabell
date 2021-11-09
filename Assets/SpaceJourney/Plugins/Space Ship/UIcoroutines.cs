using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIcoroutines : MonoBehaviour {

	public GameObject init;
	public GameObject endit;
	public GameObject loseitobj;
	public static UIcoroutines instance;
	void Start () {
		if(instance==null){
			instance = this;
		}else{
			Debug.LogError("Two UI scripts in scene!");
		}

		StartCoroutine(initialFade());
		
	}

	IEnumerator initialFade(){
		yield return new WaitForSeconds(3f);
		init.SetActive(false);

	}
	
	bool ending;
	public void GameOver(bool lose){
		if(!ending && !lose){
			StartCoroutine(win());
		}

		if(!ending && lose){
			StartCoroutine(loseit());
		}
	}

	IEnumerator loseit(){

		ending = true;
		loseitobj.SetActive(true);
		//yield return new WaitForSeconds(5f);
		//loseitobj.SetActive(false);
		yield return null;
		ending = false;

	}

	IEnumerator win(){
		ending = true;
		endit.SetActive(true);
		yield return new WaitForSeconds(5f);
		endit.SetActive(false);
		//ending = false;
	}
}