using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CMF
{
    //This script rotates all gameobjects inside the attached trigger collider around a central axis (the forward axis of this gameobject);
    //In combination with a tube-shaped collider, this script can be used to let a player walk around on the inside walls of a tunnel;
    public class GravityTunnel : MonoBehaviour
    {
        public bool smoothRotation = false;
        public float smoothRotationSpeed = 30f;

        public bool isPlayerInTonnel { private set; get; }
        //List of rigidbodies inside the attached trigger;
        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        void FixedUpdate()
        {
            for (int i = 0; i < rigidbodies.Count; i++)
            {
                //Calculate center position based on rigidbody position;
                Vector3 _center =
                    Vector3.Project((rigidbodies[i].transform.position - transform.position), ((transform.position + transform.forward) - transform.position)) + transform.position;

                if (rigidbodies[i].CompareTag("Player"))
                    RotateRigidbody(rigidbodies[i].transform, (_center - rigidbodies[i].transform.position).normalized);
                else if (rigidbodies[i].CompareTag("Block"))
                {
                    Vector3 rbPoint = transform.InverseTransformPoint(rigidbodies[i].transform.position);

                    float angle = CalculateAngle(Vector2.up, rbPoint);
                    Vector3 gravity = new Vector3(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0) * 9.81f;
                    GravityGunIteractiveObject block = rigidbodies[i].GetComponent<GravityGunIteractiveObject>();
                    block.SetGravity(gravity);

                }
            }
        }

        private float CalculateAngle(Vector3 from, Vector3 to)
        {

            return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;

        }

        void OnTriggerEnter(Collider col)
        {
            Rigidbody _rigidbody = null;
            if (col.CompareTag("Player/GravityCollider"))
            {
                isPlayerInTonnel = true;
                _rigidbody = GameManager.Player.GetComponent<PlayerReferences>().rigRigidbody;
                
            }
            else if(col.CompareTag("Block"))
            {
                _rigidbody = col.GetComponent<Rigidbody>();
            }


            if (_rigidbody)
                rigidbodies.Add(_rigidbody);

        }

        void OnTriggerExit(Collider col)
        {
            Rigidbody _rigidbody = null;
            if (col.CompareTag("Player/GravityCollider"))
            {
                isPlayerInTonnel = false;
                _rigidbody = GameManager.Player.GetComponent<PlayerReferences>().rigRigidbody;
            }
            else if(col.CompareTag("Block"))
            {
                _rigidbody = col.GetComponent<Rigidbody>();
            }


            if (_rigidbody)
            {

                rigidbodies.Remove(_rigidbody);

            }

        }


        void RotateRigidbody(Transform _transform, Vector3 _targetDirection)
        {
            //Get rigidbody component of transform;
            Rigidbody _rigidbody = _transform.GetComponent<Rigidbody>();

            _targetDirection.Normalize();

            //Calculate rotation difference;
            Quaternion _rotationDifference = Quaternion.FromToRotation(_transform.up, _targetDirection);

            //Save start and end rotation;
            Quaternion _startRotation = _transform.rotation;
            Quaternion _endRotation = _rotationDifference * _transform.rotation;


            if (smoothRotation)
            {
                _rigidbody.MoveRotation(Quaternion.RotateTowards(_rigidbody.rotation, _endRotation, smoothRotationSpeed * Time.deltaTime));
            }
            else
            {
                //Rotate rigidbody;
                _rigidbody.MoveRotation(_endRotation);
            }


        }

        //Calculate a counter rotation from a rotation;
        Quaternion GetCounterRotation(Quaternion _rotation)
        {
            Vector3 _axis;
            float _angle;

            _rotation.ToAngleAxis(out _angle, out _axis);
            Quaternion _rotationAdd = Quaternion.AngleAxis(Mathf.Sign(_angle) * 180f, _axis);

            return _rotation * Quaternion.Inverse(_rotationAdd);
        }
    }
}

