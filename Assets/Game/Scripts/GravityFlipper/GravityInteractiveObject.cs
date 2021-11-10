using UnityEngine;
using UnityEngine.Events;

namespace GravityFlipper
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ConstantForce))]
    
    public class GravityInteractiveObject : MonoBehaviour, IGravityChanged
    {
        public bool UseGravity = true;
        public Vector3 OldGravity = Physics.gravity;
        
        private ConstantForce _constantForce;
        private Rigidbody _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _constantForce = GetComponent<ConstantForce>();
        }

        public void OnGravityChanged(Vector3 gravity)
        {
            var mass = _rigidbody.mass;
            if (UseGravity)
            {
                _constantForce.force = gravity * mass;
            }
            else
            {
                _constantForce.force = Vector3.zero;
            }
        }
    }
}
