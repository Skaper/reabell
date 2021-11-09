using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG
{

    // This will rotate a transform along with a users headset. Useful for keeping an object aligned with the camera, independent of the player capsule collider.
    public class RotateWithHMD : MonoBehaviour
    {

        /// <summary>
        /// The Character Capsule to  rotate along with
        /// </summary>
        public Transform trekposition;
        //public Camera camera;

        /// <summary>
        /// Offset to apply in local space to the hmdTransform
        /// </summary>
        public Vector3 Offset = new Vector3(0, -0.25f, 0);

        public float RotateSpeed = 5f;

        public float MoveSpeed = 5f;
        Rigidbody playerRigidbody;
        //Transform trekposition;
        private void Start()
        {
            playerRigidbody = GameManager.Player.GetComponent<PlayerReferences>().rigRigidbody;
            //trekposition = GameManager.Player.transform;
        }
        void LateUpdate()
        {
            updateBodyPosition();
        }

        void updateBodyPosition()
        {


            if (trekposition != null)
            {
                transform.position = trekposition.position;
                //transform.position = trekposition.position + Offset;
                transform.localPosition -= trekposition.TransformVector(Offset);
                //Option 1
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, Camera.main.transform.rotation.eulerAngles.y, 0.0f), Time.deltaTime * RotateSpeed);

                //Optrion 2
                //Vector3 camAngle = new Vector3(playerRigidbody.rotation.eulerAngles.x, trekposition.rotation.eulerAngles.y + playerRigidbody.rotation.eulerAngles.y, playerRigidbody.rotation.eulerAngles.z);
                //Quaternion deltaRotation = Quaternion.Euler(camAngle);
                //transform.rotation = deltaRotation;

                //Option 3
                //Quaternion q = Quaternion.AngleAxis(rotationAmount, Vector3.up);
                //Quaternion initialRotation = playerRigidbody.rotation;
                //Quaternion.LookRotation();
                //Quaternion.Euler(0.0f, trekposition.rotation.eulerAngles.y, 0.0f);
                //initialRotation.y += trekposition.rotation.y;
                //transform.rotation = initialRotation;


                //Quaternion rotationQuaternion = Quaternion.Euler(
                //    new Vector3(
                //        playerRigidbody.rotation.eulerAngles.x, 
                //        trekposition.rotation.eulerAngles.y, 
                //        playerRigidbody.rotation.eulerAngles.z
                //        )
                //    );
                //
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationQuaternion, 1.0f);


                Vector3 camAngle = new Vector3(0f, trekposition.eulerAngles.y, 0f);
                Quaternion deltaRotation = Quaternion.Euler(camAngle);
                gameObject.transform.rotation = deltaRotation;

            }
        }
    }
}
