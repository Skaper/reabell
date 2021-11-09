using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
public class PlayerInSpaceController : MonoBehaviour
{
    [Header("Thrust settings")]
    public float minThrust = 0f;
    public float maxThrust = 20f;

    [Header("Speed settings")]
    public float turnSpeed = 10f;
    public float moveSpeed = 10f;

    [Header("Collision Recoil Force settings")]
    public float backRecoilForce = 20f;
    public float backForceReturnRate = 0.1f;

    [Header("Rotational inertia settings")]
    public float pitchReturnRate = 0.2f;
    public float rollReturnRate = 0.2f;
    public float yawReturnRate = 0.2f;

    public SetCameraCenter cameraRecenter;
    [HideInInspector]
    public float velocityMagnitude;
    [HideInInspector]
    public bool isActive = true;

    [HideInInspector]
    public bool isPlayerInShip = false;
    [HideInInspector]
    public bool isCockpitOpen = false;

    private Vector3 previousPosition;
    private float thrust = 0;
    private float backRecoilForceStartTime;

    private Rigidbody rb;
    private AudioSource audioEngine;
    private float audioEngineDefaultVolume;

    private float targetPitch;
    private float pitchStartTime;

    private float targetYaw;
    private float yawStartTime;

    float targetRoll;
    float rollStartTime;

    private InputBridge input;

    private void Start()
    {
        cameraRecenter.Recenter();
        input = InputBridge.Instance;
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        audioEngine = GetComponent<AudioSource>();
        audioEngine.loop = true;
        audioEngineDefaultVolume = audioEngine.volume;

        audioEngine.volume = 0f;
        audioEngine.Play();
    }
    private void Update()
    {
        if (isActive) { 
            if (rb.velocity.sqrMagnitude >= 0.001f)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, (Time.time - backRecoilForceStartTime) * backForceReturnRate);
            }

            if (input.RightGrip > 0.1f)
            {
                thrust += moveSpeed * Time.deltaTime;
            }
            else
            {
                if(thrust > 0f)thrust -= moveSpeed * Time.deltaTime;
            }

            if (input.RightGrip > 0.1f)
            {
                StartCoroutine(FadeAudioSource.StartFade(audioEngine, 1f, 0f, audioEngineDefaultVolume));
            }
            else 
            {
                StartCoroutine(FadeAudioSource.StartFade(audioEngine, 1f, audioEngineDefaultVolume, 0f));
            }
            thrust = Mathf.Clamp(thrust, minThrust, maxThrust);
        }
        else
        {
            thrust = 0f;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            CalculateRotation();
            CalculateThrust();
        }
        rb.angularVelocity = Vector3.zero;
    }

    private void CalculateRotation()
    {

        //Right controller
        float roll = -input.RightThumbstickAxis.x * Time.fixedDeltaTime * turnSpeed;
        float pitch = -input.RightThumbstickAxis.y * Time.fixedDeltaTime * turnSpeed;

        //Left controller
        float yaw = input.LeftThumbstickAxis.x * Time.fixedDeltaTime * turnSpeed;

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



        Vector3 keyboardRot = new Vector3(targetPitch, targetYaw, targetRoll);

        transform.Rotate(keyboardRot);
    }

    private void CalculateThrust()
    {
        rb.MovePosition(transform.position + transform.forward * thrust * Time.fixedDeltaTime);

        velocityMagnitude = (transform.position - previousPosition).magnitude / (Time.deltaTime * Time.deltaTime);
        previousPosition = transform.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) return;
        if (!isActive) return;
        thrust = 0;
        Vector3 dir = collision.contacts[0].point - transform.position;
        dir = -dir.normalized;
        dir += Vector3.Cross(dir, Vector3.left);
        backRecoilForceStartTime = Time.time;
        rb.AddForce(dir * backRecoilForce * velocityMagnitude);
    }

    public void DisableSound()
    {
        if(audioEngine.volume > audioEngineDefaultVolume/2f) StartCoroutine(FadeAudioSource.StartFade(audioEngine, 1f, audioEngineDefaultVolume, 0f));
    }
    public void OpenShipCockpit()
    {
        isCockpitOpen = true;
    }
    public void EnterInShip()
    {
        isPlayerInShip = true;
    }
}
