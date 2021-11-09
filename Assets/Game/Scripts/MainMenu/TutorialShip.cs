using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using BNG;

public class TutorialShip : MonoBehaviour
{
    public bool isShipActive = true;
    public bool isControlActive = true;

    public Transform backRayPoint;
    public float radius = 0.5f;

    public Collider wallCollider;

    [Header("Weapons")]
    public Transform weaponPointLeft;
    public Transform weaponPointRight;
    public Transform weaponRay;
    public int rayLayout;
    public GameObject reticle;
    public Transform weaponPointer;
    public GameObject bullet;
    public float bulletForce = 50f;
    public AudioClip[] shootClips;
    public bool playeSingleClipOnce = true;
    public AudioSource shootAudio;

    [Header("Thrust settings")]
    [SerializeField]
    private float minThrust = 5f;
    [SerializeField]
    private float maxThrust = 20f;
    [SerializeField]
    private float maxBackwardThrust = 5f;
    [SerializeField]
    private float energyDecrease = 10f;
    [SerializeField]
    private float energyLevel;
    [SerializeField]
    private float energyRecovery = 10f;
    [SerializeField]
    private float energyRegenerationDelay = 2f;


    private float maxEnergyLevel;
    private bool isEnergyRecoveryProcess;

    [Header("Speed settings")]
    public float turnSpeed = 10f;
    public float turnSpeedRoll = 20f;
    public float thrustIncreaseSpeed = 4f;
    public float thrustDecreaseSpeed = 4f;
    public float throttleSpeed = 10f;

    [Header("Collision Recoil Force settings")]
    public float backRecoilForce = 20f;
    public float backForceReturnRate = 0.1f;


    [Header("Rotational inertia settings")]
    public float pitchReturnRate = 0.2f;
    public float rollReturnRate = 0.2f;
    public float yawReturnRate = 0.2f;

    [Header("FX settings")]
    [Tooltip("nitial thrust value when it is necessary to enable the warp effect")]
    public float hightSpeedEffect = 10f;




    [Header("Sound settings")]
    public AudioClip[] impactCollisions;
    public AudioClip[] hitsOnShield;

    public AudioSource impactCollisionAudio;
    public AudioSource hitOnShieldAudio;
    public AudioSource engineAudio;
    public AudioSource navigationAudio;
    private float maxVolumeEngineAudio;
    private float maxVolumeNavigationAudio;

    public UnityEvent onHightSpeed;
    public UnityEvent onLowSpeed;

    [HideInInspector]
    public float velocityMagnitude;

    private Vector3 previousPosition;
    private float thrust = 0;
    private float backRecoilForsStartTime;

    private Rigidbody rb;
    private float targetPitch;
    private float pitchStartTime;

    private float targetYaw;
    private float yawStartTime;

    private float targetRoll;
    private float rollStartTime;

    private bool t_isShipActive = true;

    private bool isMovingKeyPressed;
    private float energyRegenerationTimer;

    private bool isLeftWeaponShoot;
    private bool isRightWeaponShoot;

    private InputBridge input;
    private enum MoveType
    {
        FORWARD,
        BACKWARD,
        ROLL,

    }
    private void Awake()
    {
        t_isShipActive = isShipActive;


        Debug.Log("PlayerShip pos: " + transform.position);
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        maxEnergyLevel = energyLevel;

        input = InputBridge.Instance;


        maxVolumeEngineAudio = engineAudio.volume;
        maxVolumeNavigationAudio = navigationAudio.volume;
        //Physics.IgnoreLayerCollision(0, 18);
        Physics.IgnoreCollision(GetComponent<MeshCollider>(), wallCollider);
    }

    private void Shooting()
    {
        if (input.LeftTriggerDown)
        {
            
            SimpleBullet simpleBullet1 = Instantiate(bullet, weaponPointLeft.position, Quaternion.identity).GetComponent<SimpleBullet>();
            simpleBullet1.AddForce(transform.forward *  bulletForce);//(Mathf.Pow(mainShipController.velocityMagnitude, 1f / 3f) + bulletForce));
            simpleBullet1.creatorTag = tag;
            isLeftWeaponShoot = true;
            
            if (input.LeftGrip > 0.7f)
            {
                
                    SimpleBullet simpleBullet2 = Instantiate(bullet, weaponPointRight.position, Quaternion.identity).GetComponent<SimpleBullet>();
                    simpleBullet2.AddForce(transform.forward *  bulletForce);
                    simpleBullet2.creatorTag = tag;
                    isRightWeaponShoot = true;
                
            }

        }

        if (isLeftWeaponShoot || isRightWeaponShoot)
        {
            isLeftWeaponShoot = false;
            isRightWeaponShoot = false;
            MakeShootSound();
        }

        if (isLeftWeaponShoot || isRightWeaponShoot)
        {
            isLeftWeaponShoot = false;
            isRightWeaponShoot = false;
            MakeShootSound();
        }
    }

    private void MakeShootSound()
    {
        if (playeSingleClipOnce)
        {
            shootAudio.clip = shootClips[Random.Range(0, shootClips.Length)];
            shootAudio.Play();
        }
        else if (!shootAudio.isPlaying)
        {
            shootAudio.clip = shootClips[Random.Range(0, shootClips.Length)];
            shootAudio.Play();
        }
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        if (isShipActive != t_isShipActive)
        {
            t_isShipActive = isShipActive;
            if (isShipActive) GameManager.Player = gameObject;
        }
        if (!isShipActive) return;

        Shooting();

        //if (!controlStand.isControlActive()) return;

        bool isBackwardThrust = false;
        float roll = 0;
        float pitch = 0;
        float throttle = 0;
        float yaw = 0;
        float inputThrust = 0;

        if (isControlActive)
        {
            //Right controller
            roll = -input.RightThumbstickAxis.x * Time.fixedDeltaTime * turnSpeedRoll;
            pitch = -input.RightThumbstickAxis.y * Time.fixedDeltaTime * turnSpeed;
            inputThrust = input.RightGrip;
            isBackwardThrust = input.AButton;

            //Left controller
            throttle = input.LeftThumbstickAxis.y * Time.fixedDeltaTime * throttleSpeed;
            if (Mathf.Abs(input.LeftThumbstickAxis.y) < 0.45f) throttle = 0f;
            yaw = input.LeftThumbstickAxis.x * Time.fixedDeltaTime * turnSpeed;
        }

        if (rb.velocity.sqrMagnitude >= 0.01f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, (Time.time - backRecoilForsStartTime) * backForceReturnRate);
        }

        rb.angularVelocity = Vector3.zero;

        if (inputThrust > 0.001f)
        {
            isMovingKeyPressed = true;
        }
        else
        {
            isMovingKeyPressed = false;
        }



        if (isMovingKeyPressed && energyLevel > 0.1f)
        {
            float target_thrust;
            if (isBackwardThrust)
            {
                target_thrust = -ReMap(inputThrust, 0.1f, 1f, 0f, maxBackwardThrust);
                if (thrust > -maxBackwardThrust)
                {
                    if (thrust > 0)
                    {
                        thrust -= thrustDecreaseSpeed * Time.deltaTime * 4f;
                    }
                    else
                    {
                        thrust -= thrustDecreaseSpeed * Time.deltaTime * 1.5f;
                    }

                }
                else
                {
                    thrust = -maxBackwardThrust;
                }
            }
            else
            {
                target_thrust = ReMap(inputThrust, 0.1f, 1f, 0f, maxThrust);
                if (thrust < maxThrust)
                {
                    thrust += thrustIncreaseSpeed * Time.deltaTime; ;
                }
                else
                {
                    thrust = maxThrust;
                }
            }


            //thrust += Time.deltaTime * turnSpeed * (rightThrust +1f);
            isEnergyRecoveryProcess = false;

            energyLevel -= energyDecrease * Time.deltaTime;
            if (energyLevel < 0) energyLevel = 0;

            energyRegenerationTimer = 0;
        }
        else if (!isMovingKeyPressed || energyLevel <= 0.1f)
        {
            if (thrust > 0) thrust -= Time.deltaTime * thrustDecreaseSpeed;
            if (energyRegenerationTimer > energyRegenerationDelay) isEnergyRecoveryProcess = true;
        }

        energyRegenerationTimer += Time.deltaTime;

        if (thrust >= hightSpeedEffect) onHightSpeed?.Invoke();
        else onLowSpeed?.Invoke();

        if (isEnergyRecoveryProcess)
        {
            if (energyLevel < maxEnergyLevel)
            {
                energyLevel += energyRecovery * Time.deltaTime;
                if (energyLevel > maxEnergyLevel)
                {
                    energyLevel = maxEnergyLevel;
                    isEnergyRecoveryProcess = false;
                }
            }
        }




        EngineAudio();
        NavigationAudio(new float[] { roll, pitch, yaw, throttle });

        CalculateRotation(roll, pitch, yaw);
        CalculateThrust(throttle);


    }

   
    private void EngineAudio()
    {
        if (thrust > 0.1f)
        {

            if (!engineAudio.isPlaying) engineAudio.Play();
            engineAudio.volume = ReMap(thrust, 0.1f, maxThrust, 0f, maxVolumeEngineAudio);
        }
        else if (thrust >= -0.1f && thrust <= 0.1f)
        {
            engineAudio.volume = 0f;
            engineAudio.Stop();
        }
        else
        {
            if (!engineAudio.isPlaying) engineAudio.Play();
            engineAudio.volume = ReMap(-thrust, 0.1f, maxBackwardThrust, 0f, maxVolumeEngineAudio);
        }
    }

    private void NavigationAudio(float[] values)
    {
        float[] valuesAbs = values.Select(x => Mathf.Abs(x)).ToArray();
        float maxValue = valuesAbs.Max();
        if (maxValue > 0.01f)
        {

            if (!navigationAudio.isPlaying) navigationAudio.Play();
            navigationAudio.volume = ReMap(maxValue, 0.01f, 0.5f, 0f, maxVolumeNavigationAudio);
        }
        else if (thrust >= -0.1f && thrust <= 0.1f)
        {
            navigationAudio.volume = 0f;
            navigationAudio.Stop();
        }
        else
        {
            if (!navigationAudio.isPlaying) navigationAudio.Play();
            navigationAudio.volume = ReMap(maxValue, 0.01f, 0.5f, 0f, maxVolumeNavigationAudio);
        }
    }

    private void CalculateRotation(float roll, float pitch, float yaw)
    {




        if (pitch != 0)
        {
            pitchStartTime = Time.time;
            targetPitch = pitch;
        }
        else
        {
            if (Mathf.Abs(targetPitch) > 0.001f) targetPitch = Mathf.Lerp(targetPitch, 0, (Time.time - pitchStartTime) * pitchReturnRate);
            else targetPitch = 0;
        }

        if (yaw != 0)
        {
            yawStartTime = Time.time;
            targetYaw = yaw;
        }
        else
        {
            if (Mathf.Abs(targetYaw) > 0.001f) targetYaw = Mathf.Lerp(targetYaw, 0, (Time.time - yawStartTime) * yawReturnRate);
            else targetYaw = 0;
        }

        if (roll != 0)
        {
            rollStartTime = Time.time;
            targetRoll = roll;
        }
        else
        {
            if (Mathf.Abs(targetRoll) > 0.001f) targetRoll = Mathf.Lerp(targetRoll, 0, (Time.time - rollStartTime) * rollReturnRate);
            else targetRoll = 0;
        }



        Vector3 rot = new Vector3(targetPitch, targetYaw, targetRoll);

        transform.Rotate(rot);

    }

    private void CalculateThrust(float throttle)
    {

        thrust = Mathf.Clamp(thrust, -maxBackwardThrust, maxThrust);

        
        rb.MovePosition(transform.position + transform.forward * thrust * Time.fixedDeltaTime + transform.up * throttle);


        velocityMagnitude = 2 * (transform.position - previousPosition).magnitude / (Time.deltaTime * Time.deltaTime); //TODO velocity
        previousPosition = transform.position;
        if (velocityMagnitude > 4000f) velocityMagnitude = 4000f;
    }

    private void OnCollisionStay(Collision collision)
    {
       
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isShipActive) return;
        if (collision.gameObject.CompareTag("Ignore")) return;
        if (collision.gameObject.GetComponent<Rigidbody>() != null) return;
        thrust = 0;
        Vector3 dir = collision.contacts[0].point - transform.position;
        dir = -dir.normalized;
        //dir += Vector3.Cross(dir, Vector3.left);
        backRecoilForsStartTime = Time.time;
        Vector3 force = dir * backRecoilForce * velocityMagnitude;
        rb.AddForce(force);

        if (!impactCollisionAudio.isPlaying)
        {
            impactCollisionAudio.clip = impactCollisions[Random.Range(0, impactCollisions.Length)];
            impactCollisionAudio.Play();
        }
    }

    private void OnBulletHit()
    {
        if (!hitOnShieldAudio.isPlaying)
        {
            hitOnShieldAudio.clip = hitsOnShield[Random.Range(0, hitsOnShield.Length)];
            hitOnShieldAudio.Play();
        }
    }

    public float GetEnergyLevel()
    {
        return energyLevel;
    }

    public float GetMaxEnergyLevel()
    {
        return maxEnergyLevel;
    }

    public float GetThrust()
    {
        return thrust;
    }
    public float GetMaxThrust()
    {
        return maxThrust;
    }

    private float ReMap(float x, float in_min, float in_max, float out_min, float out_max)
    {
        if (x < in_min) return out_min;
        if (x > in_max) return out_max;
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
}
