using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GravityFlipper
{
    public class GravityFlip : MonoBehaviour
    {
        public GravityTunnel gravityTunnel;

        public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        public Vector3 angle = Vector3.zero;

        public bool isExitGravityZero = true;

        private Rigidbody playerRigidbody;
        private bool playerChengedGravity;
        
        private void Start()
        {
            playerRigidbody = GameManager.Player.GetComponent<PlayerReferences>().rigRigidbody; 
        }

        void OnTriggerStay(Collider col)
        {
            if (col.CompareTag("Player/GravityCollider"))
            {
                if (gravityTunnel && !gravityTunnel.isPlayerInTonnel)
                {
                    flipGravity();
                    playerChengedGravity = true;
                }
            }else if (col.CompareTag("Block"))
            {
                col.GetComponent<IGravityChanged>().OnGravityChanged(gravity);
            }
        }

      
        private void flipGravity()
        {
            var targetGravity = Quaternion.Euler(angle) * Vector3.up;
            RotateRigidbody(playerRigidbody.transform, targetGravity.normalized);
        }
        
        void RotateRigidbody(Transform _transform, Vector3 _targetDirection)
        {
            //Get rigidbody component of transform;
            Rigidbody _rigidbody = _transform.GetComponent<Rigidbody>();

            _targetDirection.Normalize();

            //Calculate rotation difference;
            Quaternion _rotationDifference = Quaternion.FromToRotation(_transform.up, _targetDirection);

            //Save start and end rotation;
            Quaternion _endRotation = _rotationDifference * _transform.rotation;

            _rigidbody.MoveRotation(_endRotation);
        }
        
        private void OnTriggerExit(Collider other)
        {

            if (other.CompareTag("Block") && isExitGravityZero)
            {
                other.GetComponent<IGravityChanged>().OnGravityChanged(gravity);
            }else if (other.CompareTag("Player/GravityCollider"))
            {
                playerChengedGravity = false;
            }
        }

    }
}

