using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : MonoBehaviour
{
    [SerializeField] private VRPointer pointer = null;


    public Transform holdPos;
    public float attractSpeed;

    public float minThrowForce;
    public float maxThrowForce;

    private GameObject objectIHave;
    private Vector3 objectOriginalScale;
    private GravityGunIteractiveObject iteractiveObject;
    private Rigidbody objectRB;
    private float throwForce;
    private Vector3 rotateVector = Vector3.one;

    public bool hasObject { get; private set; }
    private bool stackForce = false;

    //Audio setup
    public AudioClip gunStartSound;
    public AudioClip gunLoopSound;
    public AudioClip gunReverseLoopSound;
    public AudioClip gunShootSound;
    public AudioClip gunEndSound;

    private bool playShootSound = false;
    private AudioSource audioSource;
    
    private void Start()
    {
        throwForce = minThrowForce;
        
        audioSource = GetComponent<AudioSource>();

    }
    private void Update()
    {
        if (hasObject)
        {
            if (objectIHave.tag == "Untagged")
            {
                removeObject();
            }
        }

        

        if (Input.GetKeyDown(InputManager.Attack2))
        {
            if (throwForce > 0.6f)  audioSource.clip = gunReverseLoopSound;
        }
        if (Input.GetKeyUp(InputManager.Attack2) && hasObject)
        {
            if (throwForce >= maxThrowForce / 2f) playShootSound = true;
            else playShootSound = false;
            ShootObj();
        }
        if (Input.GetKeyDown(InputManager.Attack1) && !hasObject)
        {
            CheckRayCastObject();
            audioSource.Stop();
            stackForce = false;
        }

        if (Input.GetKey(InputManager.Attack2) && hasObject)
        {
            throwForce += 0.2f;
            stackForce = true;
            playReverseLoopSound();
        }
    }

    private void FixedUpdate()
    {
        if (hasObject)
        {
            if (!stackForce) playLoopSound();

            if (iteractiveObject.IsContactWithPlayer())
            {
                DropObj();
            }
            else
            {
                float dist = CheckDist();
                if (dist > 0.001f && dist <= pointer.defaultLength*1.5f)
                {
                    MoveObjToPos(dist);
                }
                else if(dist > pointer.defaultLength * 1.5f)
                {
                    DropObj();
                }
            }
            objectRB.velocity = GameManager.Player.GetComponent<Rigidbody>().velocity;
        }
    }


    private void removeObject()
    {
        objectIHave.transform.localScale = objectOriginalScale;
        objectIHave.transform.SetParent(null);
        objectIHave = null;
        iteractiveObject = null;
        hasObject = false;
        stackForce = false;
        audioSource.Stop();
        audioSource.loop = false;
    }

    private void CalculateRotVector()
    {
        float x = Random.Range(-0.5f, 0.5f);
        float y = Random.Range(-0.5f, 0.5f);
        float z = Random.Range(-0.5f, 0.5f);

        rotateVector = new Vector3(x, y, z);
    }

    private void RotateObj()
    {
        objectIHave.transform.Rotate(rotateVector);
    }



    public float CheckDist()
    {
        float dist = Vector3.Distance(objectIHave.transform.position, holdPos.transform.position);
        return dist;
    }

    private void MoveObjToPos(float dist)
    {
        //float speed = Mathf.Pow((pointer.defaultLength - dist), 0.5f) * attractSpeed;
        
        
        objectIHave.transform.position = Vector3.Lerp(objectIHave.transform.position, holdPos.position, attractSpeed * Time.fixedDeltaTime); //attractSpeed
        //GameManager.Player.GetComponent<Rigidbody>().velocity;
    }

    public void DropObj()
    {
        if (!hasObject) return;
        iteractiveObject.EnableGravity();
        iteractiveObject.inGravityGun = false;
        iteractiveObject = null;
        objectIHave.transform.SetParent(null);
        objectIHave.transform.localScale = objectOriginalScale;
        objectIHave = null;
        hasObject = false;
        stackForce = false;
        if (playShootSound)
        {
            audioSource.clip = gunShootSound;
        }
        else
        {
            audioSource.clip = gunEndSound;
        }
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.Play();

    }

    private void ShootObj()
    {
        throwForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);

        objectRB.AddForce(pointer.Camera.transform.forward * throwForce, ForceMode.Impulse);
        objectRB.AddTorque(Vector3.one * throwForce, ForceMode.VelocityChange);
        throwForce = minThrowForce;
        DropObj();
    }

    private void CheckRayCastObject()
    {
        GameObject target = pointer.Target;
        if(target != null)
        {
            if (target.tag != "Block") return;
            iteractiveObject = target.GetComponent<GravityGunIteractiveObject>();
            if (iteractiveObject)
            {
                
                objectIHave = target;
                objectOriginalScale = objectIHave.transform.localScale;
                objectIHave.transform.SetParent(holdPos.transform);
                objectIHave.transform.localScale = objectOriginalScale;
                objectRB = objectIHave.GetComponent<Rigidbody>();
                objectRB.angularVelocity = Vector3.zero;
                objectRB.velocity = Vector3.zero;

                iteractiveObject.inGravityGun = true;
                
                //objectRB.constraints = RigidbodyConstraints.FreezeRotation;
                
                objectRB.useGravity = false;
                iteractiveObject.DisableGravity();
                hasObject = true;
                
                CalculateRotVector();
            }
        }
    }

    private void playLoopSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = gunLoopSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    private void playReverseLoopSound()
    {
        if (audioSource.clip == gunReverseLoopSound)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = gunReverseLoopSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}