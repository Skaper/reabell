using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeleport : MonoBehaviour
{
    public float maxDistanceFromCenter = 50f;
    public UnityEvent onTeleport;


    private GameObject spawnPoint;
    private Transform centralPoint;
    
    private float fallDownTimer;
    private float maxTimeToFallDown = 0.5f;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        centralPoint = GameManager.WorldCenterPoint.transform;
        spawnPoint = GameManager.PlayerSpawnPoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, centralPoint.position);
        if (dist > maxDistanceFromCenter)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.MoveRotation(spawnPoint.GetComponent<TeleporterGravityFlip>().getGravityRotation());
            Debug.Log("Teleport. Dist: " + dist);
            teleport(spawnPoint.transform.position);
            fallDownTimer += Time.fixedDeltaTime;

        }
        if (Input.GetKeyDown(InputManager.Action4))
        {
            teleportToSpawn();
        }
    }
    public void teleportToSpawn()
    {
        transform.position = spawnPoint.transform.position;
    }

    public void teleport(Vector3 position)
    {
        if (fallDownTimer > maxTimeToFallDown)
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            transform.position = position;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            fallDownTimer = 0;
            onTeleport?.Invoke();
        }

       

    }

}
