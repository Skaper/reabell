using UnityEngine;

namespace GravityFlipper
{
    public class GravityFlip : MonoBehaviour
    {
        public GravityTunnel gravityTunnel;

        public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        public Vector3 angle = Vector3.zero;

        public bool isExitGravityZero = true;

        private PlayerReferences _playerReferences;
        private bool playerChengedGravity;

        private void Awake()
        {
            _playerReferences = GameManager.PlayerReferences;
        }

        void OnTriggerStay(Collider col)
        {
            if (!playerChengedGravity && col.CompareTag("Player/GravityCollider"))
            {
                if (gravityTunnel && gravityTunnel.isPlayerInTonnel)
                {
                    return;
                }

                _playerReferences.PlayerWalkerController.Gravity = GetMaxGravity();
                FlipGravity(_playerReferences.transform);
                
                playerChengedGravity = true;
            }
            else
            {
                var gravityChanged = col.GetComponent<IGravityChanged>();
                gravityChanged?.OnGravityChanged(gravity);
            }
        }

      
        private void FlipGravity(Transform tr)
        {
            var targetGravity = Quaternion.Euler(angle) * Vector3.up;
            RotateRigidbody(tr, targetGravity.normalized);
        }
        
        void RotateRigidbody(Transform tr, Vector3 targetDirection)
        {
            //Get rigidbody component of transform;
            Rigidbody _rigidbody = tr.GetComponent<Rigidbody>();

            targetDirection.Normalize();

            //Calculate rotation difference;
            Quaternion _rotationDifference = Quaternion.FromToRotation(tr.up, targetDirection);

            //Save start and end rotation;
            Quaternion _endRotation = _rotationDifference * tr.rotation;

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
        
        private float GetMaxGravity(){
            return Mathf.Max(Mathf.Max(gravity.x, gravity.y), gravity.z);
        }

    }
}

