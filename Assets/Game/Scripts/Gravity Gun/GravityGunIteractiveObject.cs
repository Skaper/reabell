using UnityEngine;
using BNG;
using GravityFlipper;

public class GravityGunIteractiveObject : MonoBehaviour, IGravityChanged
{
    [SerializeField] private bool _isPlayerCollision = false;

    public bool returnAfterFall = false;
    public bool inGravityGun = false;
    public bool useMass = true;
    public bool useGravity = true;
    public Collider collider;

    private Rigidbody _rigidbody;
    private Vector3 _startPosition;
    private Vector3 _oldGravity = Physics.gravity;
    private ConstantForce _constantForce;
    private Grabbable _grabbable;
    private bool _inHand = false;

    public bool isCollisionStay { get; private set; }

    void Awake()
    {
        _startPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _constantForce = GetComponent<ConstantForce>();
        collider = GetComponent<Collider>();
        _grabbable = GetComponent<Grabbable>();
    }

    private void Update()
    {
        if (_grabbable)
        {
            useGravity = !_grabbable.BeingHeld;
        }

        if (Vector3.Distance(transform.position, GameManager.WorldCenterPoint.transform.position) > GameManager.LevelRadius)
        {
            if (returnAfterFall)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                transform.position = _startPosition;
            }
            else Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && Time.realtimeSinceStartup > 0.6f)
        {
            _isPlayerCollision = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            _isPlayerCollision = false;
        }
    }

    public bool IsContactWithPlayer()
    {
        return _isPlayerCollision;
    }


    public void DisablePhysics()
    {
        gameObject.isStatic = true;
        gameObject.tag = "Untagged";


        GetComponent<GravityGunIteractiveObject>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<MeshCollider>().enabled = false;
    }

    public void EnablePhysics()
    {
        gameObject.isStatic = false;
        gameObject.tag = "Block";


        GetComponent<GravityGunIteractiveObject>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<MeshCollider>().enabled = true;
    }

    public void DisableGravity()
    {
        if (_constantForce.force.Equals(Vector3.zero)) return;
        _oldGravity = _constantForce.force / _rigidbody.mass;
        _constantForce.force = Vector3.zero;
        useGravity = false;
    }

    public void EnableGravity()
    {
        if (_constantForce.force.Equals(_oldGravity)) return;
        collider.enabled = true;
        _constantForce.force = _oldGravity * _rigidbody.mass;
        useGravity = true;
    }
    
    private void OnCollisionStay(Collision collision)
    {
        isCollisionStay = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollisionStay = false;
    }

    public void OnGravityChanged(Vector3 gravity)
    {
        var mass = _rigidbody.mass;
        _oldGravity = gravity;
        if (useGravity)
        {
            _constantForce.force = _oldGravity * mass;
        }
        else _constantForce.force = Vector3.zero;
    }
}