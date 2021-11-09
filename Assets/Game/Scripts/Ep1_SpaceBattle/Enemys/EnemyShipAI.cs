using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipAI : MonoBehaviour
{
    public Action<EnemyShipAI> onDead;

    public EnemyShipWeapon aiWeapon;
    public GameObject ship;


    public Transform UpRay;
    public Transform DownRay;
    public Transform LeftRay;
    public Transform RightRay;


    public float minDistanceToTarget = 20f;
    public float speed = 2.75f;

    public float sensDistance = 10f;
    public float normalTurnSpeed = 10f;

    public GameObject target;

    private Hits hits;
    private Moving moving;


    public float rotateAngle;
    private int layerMask = 1 << 8;


    [Header("Collider Settings")]
    public float radiusMax = 20f;
    public float distMin = 50f;
    public float distMax = 300f;

    private float dist;
    private float radiusMin;
    private float linearK;
    private float linearB;
    private SphereCollider sphereCollider;


    private EnemyShipAIHealth aiHealth;
    private bool canShoot;

    private bool hasSendDeadSignal;

    private float freeWayTimer;
    private float freeWayDelay = 2f;

    private bool moveOnOrbit;


    [HideInInspector]
    public bool IsDead { get; private set; }
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        aiHealth = GetComponent<EnemyShipAIHealth>();
    }

    void Start()
    {
        if(target == null) target = GameManager.Player;
        layerMask = ~layerMask;
        //minDistanceToTarget *= minDistanceToTarget;
        aiWeapon.SetTarget(target);
        canShoot = aiWeapon.canShoot;

        //Collider Setup
        sphereCollider.isTrigger = true;
        radiusMin = sphereCollider.radius;
        linearK = (radiusMin - radiusMax) / (distMin - distMax);
        linearB = radiusMin - linearK * distMin;
        aiWeapon.canShoot = true;
    }
    


    
    
    void Update()
    {
        

    }
    void FixedUpdate()
    {
        if(aiHealth.currentHealth > 0)
        {
            FindWay();
            RotateToTarget();
            ResizeCollider();
        }
        else
        {
            IsDead = true;
            
            if (!hasSendDeadSignal)
            {
                onDead?.Invoke(this);
                aiHealth.onDead?.Invoke(null);
                hasSendDeadSignal = true;
            }
            if (canShoot)
            {
                canShoot = false;
                aiWeapon.canShoot = false;
                ship.SetActive(false);
            }
        }

        transform.Translate(transform.forward * speed, Space.World);

        

    }

    private void RotateToTarget()
    {
        Vector3 targetDir = (target.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.identity;
        float currentDist = Vector3.Distance(target.transform.position, transform.position);//(target.transform.position - transform.position).sqrMagnitude;


        if (currentDist <= minDistanceToTarget)
        {
            moveOnOrbit = true;
        }
        else if (currentDist >= minDistanceToTarget * 1.3f)
        {
            moveOnOrbit = false;
        }

        if (moveOnOrbit)
        {
            targetRotation = (Quaternion.LookRotation(Vector3.Cross(targetDir, Vector3.up)) * Quaternion.Euler(hits.targetAngle));
        }
        else
        {
            targetRotation = Quaternion.LookRotation(targetDir) * Quaternion.Euler(hits.targetAngle);
        }
        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        targetRotation,
                        Time.deltaTime * normalTurnSpeed * speed
                    );
        }


    }


    private void ResizeCollider()
    {
        dist = (target.transform.position - transform.position).sqrMagnitude;
        if (dist <= distMin * distMin)
        {
            sphereCollider.radius = radiusMin;
        }
        else if (dist >= distMax * distMax)
        {
            sphereCollider.radius = radiusMax;
        }
        else
        {
            sphereCollider.radius = linearK * Mathf.Sqrt(dist) + linearB;
        }
    }
    private void FindWay()
    {
        

        RaycastHit upHit;
        Physics.Raycast(UpRay.position, UpRay.TransformDirection(Vector3.forward), out upHit, sensDistance * 2f, layerMask, QueryTriggerInteraction.Ignore);



        if (upHit.collider)
        {
            
            hits.upDist = upHit.distance;
            if (upHit.distance <= sensDistance)
            {
                hits.targetAngle.x += rotateAngle;// map(hits.upDist, 0f, sensDistance, 1f, rotateAngle);//rotateAngle;
                Debug.DrawRay(UpRay.position, UpRay.TransformDirection(Vector3.forward) * upHit.distance, Color.red);
            }
        }
        else
        {
            if(hits.targetAngle.x >= rotateAngle / 2f) hits.targetAngle.x -= rotateAngle / 2f;

            Debug.DrawRay(UpRay.position, UpRay.TransformDirection(Vector3.forward) * 1000, Color.white);
            hits.upDist = Mathf.Infinity;
        }

        RaycastHit downHit;
        Physics.Raycast(DownRay.position, DownRay.TransformDirection(Vector3.forward), out downHit, sensDistance * 2f, layerMask, QueryTriggerInteraction.Ignore);

        if (downHit.collider)
        {
            
            hits.downDist = downHit.distance;
            if (downHit.distance <= sensDistance)
            {
                hits.targetAngle.x -= rotateAngle;//map(hits.downDist, 0f, sensDistance, 1f, rotateAngle);//rotateAngle;
                Debug.DrawRay(DownRay.position, DownRay.TransformDirection(Vector3.forward) * downHit.distance, Color.red);
            }


        }
        else
        {
            if (hits.targetAngle.x <= -rotateAngle / 2f) hits.targetAngle.x += rotateAngle/2f;

            Debug.DrawRay(DownRay.position, DownRay.TransformDirection(Vector3.forward) * 1000, Color.white);
            hits.downDist = Mathf.Infinity;
        }


        RaycastHit leftHit;
        Physics.Raycast(LeftRay.position, LeftRay.TransformDirection(Vector3.forward), out leftHit, sensDistance * 2f, layerMask, QueryTriggerInteraction.Ignore);

        if (leftHit.collider)
        {
            
            hits.leftDist = leftHit.distance;
            if (leftHit.distance <= sensDistance)
            {
                hits.targetAngle.y += rotateAngle;//map(hits.leftDist, 0f, sensDistance, 1f, rotateAngle);//rotateAngle;
                Debug.DrawRay(LeftRay.position, LeftRay.TransformDirection(Vector3.forward) * leftHit.distance, Color.red);
            }

        }
        else
        {
            if (hits.targetAngle.y >= rotateAngle / 2f) hits.targetAngle.y -= rotateAngle / 2f;

            Debug.DrawRay(LeftRay.position, LeftRay.TransformDirection(Vector3.forward) * 1000, Color.white);
            hits.leftDist = Mathf.Infinity;
        }

        RaycastHit rightHit;
        Physics.Raycast(RightRay.position, RightRay.TransformDirection(Vector3.forward), out rightHit, sensDistance*2f, layerMask, QueryTriggerInteraction.Ignore);

        if (rightHit.collider)
        {

            hits.rightDist = leftHit.distance;
            if (rightHit.distance <= sensDistance)
            {
                hits.targetAngle.y -= rotateAngle;// map(hits.rightDist, 0f, sensDistance, 1f, rotateAngle);//rotateAngle;
                Debug.DrawRay(RightRay.position, RightRay.TransformDirection(Vector3.forward) * rightHit.distance, Color.red);
            }
        }
        else
        {
            Debug.DrawRay(RightRay.position, RightRay.TransformDirection(Vector3.forward) * 1000, Color.white);
            hits.rightDist = Mathf.Infinity;

            if (hits.targetAngle.y <= -rotateAngle / 2f) hits.targetAngle.y += rotateAngle / 2f;
        }

        if (hits.leftDist < hits.rightDist) hits.freeWay = rightHit.point;

        if(hits.leftDist <= sensDistance && hits.rightDist <= sensDistance && Mathf.Abs(hits.leftDist - hits.rightDist) < 1f)
        {
            hits.targetAngle.y -= rotateAngle;//map(hits.rightDist, 0f, sensDistance, 1f, rotateAngle);//rotateAngle;
        }

        if (hits.downDist <= sensDistance && hits.upDist <= sensDistance && Mathf.Abs(hits.downDist - hits.upDist) < 1f)
        {
            hits.targetAngle.x += rotateAngle;//map(hits.downDist, 0f, sensDistance, 1f, rotateAngle);//
        }

    }
    
    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private struct Moving
    {
        public Direction direction;
        

    }
    private struct Hits
    {
        public Vector3 freeWay;
        public Vector3 targetAngle;
        public float upDist;
        public Vector3 upHitPoint;
        public float downDist;
        public Vector3 downHitPoint;
        public float leftDist;
        public Vector3 leftHitPoint;
        public float rightDist;
        public Vector3 rightHitPoint;

        public void Reload()
        {
            upDist = Mathf.Infinity;
            downDist = Mathf.Infinity;
            leftDist = Mathf.Infinity;
            rightDist = Mathf.Infinity;
        }
    }
}
