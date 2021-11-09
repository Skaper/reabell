using UnityEngine;
using BNG;
public class GravityGunIteractiveObject : MonoBehaviour
{

    [SerializeField]private bool isPlayerCollision = false;

    public bool returnAfterFall = false;
    private Vector3 startPosition;

   
    private Rigidbody rigidbody;

    public bool inGravityGun = false;
    public bool useMass = true;
    public bool useGravity = true;
    private Vector3 oldGravity = Physics.gravity;
    private ConstantForce constantForce;

    public Collider collider;

    private Grabbable grabbable;
    private bool inHand = false;

    public bool isCollisionStay { get; private set; }

    void Start()
    {
        startPosition = transform.position;
        rigidbody = GetComponent<Rigidbody>();
        constantForce = GetComponent<ConstantForce>();
        rigidbody.useGravity = false;
        collider = GetComponent<Collider>();

        grabbable = GetComponent<Grabbable>();

    }

    private void Update()
    {
        if (grabbable)
        {
            useGravity = !grabbable.BeingHeld;
        }

        if (Vector3.Distance(transform.position, GameManager.WorldCenterPoint.transform.position) > GameManager.LevelRadius)
        {
            if (returnAfterFall)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                transform.position = startPosition;
            }
            else Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && Time.realtimeSinceStartup > 0.6f)
        {
            isPlayerCollision = true;
            
        }

    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            isPlayerCollision = false;
        }
    }
    public bool IsContactWithPlayer()
    {
        return isPlayerCollision;
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
        if (constantForce.force.Equals(Vector3.zero)) return;
        oldGravity = constantForce.force;
        constantForce.force = Vector3.zero;
        useGravity = false;
    }

    public void EnableGravity()
    {
        if (constantForce.force.Equals(oldGravity)) return;
        collider.enabled = true;
        constantForce.force = oldGravity;
        useGravity = true;
    }

    public void SetGravity(Vector3 value)
    {
        oldGravity = value;
        if (useGravity)
        {

            constantForce.force = oldGravity * rigidbody.mass;

        }
        else constantForce.force = Vector3.zero;
    }

    private void OnCollisionStay(Collision collision)
    {
        isCollisionStay = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollisionStay = false;
    }
}
