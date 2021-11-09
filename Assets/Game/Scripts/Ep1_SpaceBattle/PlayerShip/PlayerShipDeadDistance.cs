
using UnityEngine;
using UnityEngine.Events;
public class PlayerShipDeadDistance : MonoBehaviour
{
    [Tooltip("Maximum distance which player can fly of this point")]
    public float maxDistance = 6500;
    [Tooltip("Maximum distance which player can fly of this point and after that his died")]
    public float maxDeadDistance = 7000;


    public UnityEvent onDangerousDistance;
    public UnityEvent onDeadDistance;

    private bool isAttentionSended = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float distance = CheckDist();
        if (!isAttentionSended && distance >= maxDistance)
        {
            isAttentionSended = true;
            onDangerousDistance?.Invoke();
        }
        if (distance > maxDeadDistance)
        {
            onDeadDistance?.Invoke();
        }

    }

    public float CheckDist()
    {
        float dist = Vector3.Distance(transform.position, Vector3.zero);
        return dist;
    }
}
