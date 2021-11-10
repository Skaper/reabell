using UnityEngine;
using System.Collections.Generic;

namespace GravityFlipper
{
    public class GravityTunnel : MonoBehaviour
    {
        public bool smoothRotation = false;
        public float smoothRotationSpeed = 30f;
        public float Gravity = -9.81f;

        public Transform Origin;

        public bool isPlayerInTonnel { private set; get; }

        //List of rigidbodies inside the attached trigger;
        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        void Update()
        {
            for (int i = 0; i < rigidbodies.Count; i++)
            {
                //Calculate center position based on rigidbody position;
                Transform targetOrigin;
                targetOrigin = Origin != null ? Origin : transform;
                
                var onNormal = ((targetOrigin.position + transform.forward) - targetOrigin.position);
                var center = Vector3.Project((rigidbodies[i].transform.position - targetOrigin.position), onNormal) + targetOrigin.position;

                if (rigidbodies[i].CompareTag("Player"))
                {
                    RotateRigidbody(rigidbodies[i].transform, (center - rigidbodies[i].transform.position).normalized); 
                }
                else
                {
                    if (rigidbodies[i].isKinematic) continue;
                    
                    var target = transform.InverseTransformPoint(rigidbodies[i].transform.position);
                    var angle = CalculateAngle(Vector3.up, target);
                    var gravity = new Vector3(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0) * -Gravity;
                        
                    var gravityChanged = rigidbodies[i].GetComponent<IGravityChanged>();
                    gravityChanged?.OnGravityChanged(gravity);
                }
            }
        }

        private float CalculateAngle(Vector3 from, Vector3 to)
        {
            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
        }

        void OnTriggerEnter(Collider col)
        {
            Rigidbody rigidbody = null;
            if (col.CompareTag("Player/GravityCollider"))
            {
                isPlayerInTonnel = true;
                rigidbody = GameManager.Player.GetComponent<PlayerReferences>().rigRigidbody;
            }
            else
            {
                rigidbody = col.GetComponent<Rigidbody>();
            }

            if (rigidbody)
            {
                rigidbodies.Add(rigidbody);
            }
        }

        void OnTriggerExit(Collider col)
        {
            Rigidbody rigidbody = null;
            if (col.CompareTag("Player/GravityCollider"))
            {
                isPlayerInTonnel = false;
                rigidbody = GameManager.Player.GetComponent<PlayerReferences>().rigRigidbody;
            }
            else
            {
                rigidbody = col.GetComponent<Rigidbody>();
            }
            
            if (rigidbody)
            {
                rigidbodies.Remove(rigidbody);
            }
        }

        void RotateRigidbody(Transform _transform, Vector3 _targetDirection)
        {
            //Get rigidbody component of transform;
            var _rigidbody = _transform.GetComponent<Rigidbody>();

            _targetDirection.Normalize();

            //Calculate rotation difference;
            Quaternion _rotationDifference = Quaternion.FromToRotation(_transform.up, _targetDirection);

            //Save start and end rotation;
            Quaternion _endRotation = _rotationDifference * _transform.rotation;
            
            if (smoothRotation)
            {
                _rigidbody.MoveRotation(Quaternion.RotateTowards(_rigidbody.rotation, _endRotation, smoothRotationSpeed * Time.deltaTime));
            }
            else
            {
                _rigidbody.MoveRotation(_endRotation);
            }
        }
    }
}