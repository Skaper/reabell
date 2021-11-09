using System.Collections;

using System.Collections.Generic;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GameVR {
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public class PlayerController : MonoBehaviour {
	
	[Header("Player Settings")]
	[Space(5)]
	public float speed = 25.0f;
	public float gravity = 150.0f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 20.0f;
	public bool canJump = true;
	public float groundDistance = 1f;
	[Space(10)]

	[Header("Main Camera")]
	[Space(5)]
	public Transform cameraTransform;
	[Space(10)]
	[Space(5)]

	private bool grounded = false;
	private GameObject player;
	private GameObject body;
	private Rigidbody rigidbody;

    private GameObject gvrEditorEmulator;
    private GameObject gvrControllerMain;

	void Awake () {
		
		body = GameObject.Find ("Body");
		rigidbody = body.GetComponent<Rigidbody> ();
		player = GameObject.Find ("Player");
		rigidbody.useGravity = true;

        gvrControllerMain = GameObject.Find("GvrControllerMain");
        gvrEditorEmulator = GameObject.Find("GvrEditorEmulator");

        if (Application.platform != RuntimePlatform.Android)
        {
            gvrControllerMain.SetActive(false);
            gvrEditorEmulator.SetActive(false);
        }
    }

	void FixedUpdate () {
		
		if (iSGrounded()) {
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3 ();


			Vector3 velocity = rigidbody.velocity;
			// Jump
			if (canJump && Input.GetButton ("Jump")) {
				rigidbody.velocity = new Vector3 (Input.GetAxis ("Horizontal"), CalculateJumpVerticalSpeed (), Input.GetAxis ("Vertical"));
			} else {
				targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			}
			targetVelocity = transform.TransformDirection (targetVelocity);
			targetVelocity *= speed;
			// Apply a force that attempts to reach our target velocity

			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rigidbody.AddForce (velocityChange, ForceMode.VelocityChange);


		} else {
			
			Vector3 velocity = rigidbody.velocity;
			Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));



			targetVelocity = transform.TransformDirection (targetVelocity);
			targetVelocity *= speed;

			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rigidbody.AddForce (velocityChange, ForceMode.VelocityChange);
		}

	
		Vector3 camAngle = new Vector3 (0f, cameraTransform.eulerAngles.y, 0f);
		Quaternion deltaRotation = Quaternion.Euler (camAngle);
		rigidbody.MoveRotation (deltaRotation);

		rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
		//player.transform.rotation = camAngle;// Quaternion.Euler(camAngle);
		grounded = false; 
	}

	void OnCollisionStay () {
		grounded = true; 
	}
	bool iSGrounded(){
		return Physics.Raycast (transform.position, -Vector3.up, groundDistance + 0.1f);
	}
	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}



}
}
