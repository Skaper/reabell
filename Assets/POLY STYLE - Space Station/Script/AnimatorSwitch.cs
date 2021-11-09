using UnityEngine;
using System.Collections;

public class AnimatorSwitch : MonoBehaviour {
    private Animator animator ;
	// Use this for initialization
	void Start () {
	animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void A1 () {
	animator.Play("Shot-01");
	}
	void A2 () {
	animator.Play("Shot-02");
	}
	void A3 () {
	animator.Play("Run");
	}
	void A4 () {
	animator.Play("Walk");
	}
	void A5 () {
	animator.Play("Defense");
	}
	void A6 () {
	animator.Play("Hit");
	}
	void A7 () {
	animator.Play("Death-01");
	}
	void A8 () {
	animator.Play("Death-02");
	}
	void A9 () {
	animator.Play("Shot-03");
	}
	void A10 () {
	animator.Play("Shot-04");
	}
	void A11 () {
	animator.Play("Reload-01");
	}
	void A12 () {
	animator.Play("Idle");
	}
}
