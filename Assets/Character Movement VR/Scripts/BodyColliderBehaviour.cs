using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BodyColliderBehaviour : MonoBehaviour
{
    public Transform Transform { get; private set; }
    public Transform FloorOffset;
    public CapsuleCollider CapsuleCollider;
    public CapsuleCollider CapsuleColliderTriger;
    public float MinColliderRadius = 0.08f;
    public float MinTriggerRadius = 0.09f;

    public delegate void CollisionStatusHandler(Transform transform, bool colliding);

    public event CollisionStatusHandler CollisionStatusChanged;

    public delegate void BlockStatusHandler(bool isBlock);

    public event BlockStatusHandler BlockStatusChanged;

    public bool IsHeightChangeBlock { get; private set; }

    private float _defaultColliderRadius;
    private float _defaultTriggerRadius;

    private void Awake()
    {
        Transform = transform;
        _defaultColliderRadius = CapsuleCollider.radius;
        _defaultTriggerRadius = CapsuleColliderTriger.radius;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHeightChangeBlock)
        {
            ResizeColliders();
        }
    }

    private void ResizeColliders()
    {
        CapsuleCollider.height = FloorOffset.localPosition.y + Transform.localPosition.y + CapsuleCollider.radius;
        CapsuleCollider.center = new Vector3(0, -(CapsuleCollider.height / 2) + CapsuleCollider.radius, 0);

        CapsuleColliderTriger.height =
            FloorOffset.localPosition.y + Transform.localPosition.y + CapsuleColliderTriger.radius;
        CapsuleColliderTriger.center =
            new Vector3(0, -(CapsuleColliderTriger.height / 2) + CapsuleColliderTriger.radius, 0);
    }

    public void BlockHeightChange(float fixedHeight = 0f)
    {
        IsHeightChangeBlock = true;
        BlockStatusChanged?.Invoke(IsHeightChangeBlock);

        CapsuleCollider.radius = MinColliderRadius;
        CapsuleColliderTriger.radius = MinTriggerRadius;

        CapsuleCollider.height = fixedHeight;
        CapsuleCollider.center = Vector3.zero;

        CapsuleColliderTriger.height = fixedHeight;
        CapsuleColliderTriger.center = Vector3.zero;
    }

    public void UnblockHeightChange()
    {
        if (!IsHeightChangeBlock)
        {
            return;
        }

        CapsuleCollider.radius = _defaultColliderRadius;
        CapsuleColliderTriger.radius = _defaultTriggerRadius;
        StartCoroutine(SmoothUp(0.3f));
    }

    private IEnumerator SmoothUp(float time)
    {
        var finalHeight = FloorOffset.localPosition.y + Transform.localPosition.y + CapsuleCollider.radius;
        var finalHeightTrigger = FloorOffset.localPosition.y + Transform.localPosition.y + CapsuleColliderTriger.radius;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            CapsuleCollider.height = Mathf.Lerp(0, finalHeight, (elapsedTime / time));
            CapsuleCollider.center = new Vector3(0, -(CapsuleCollider.height / 2) + CapsuleCollider.radius, 0);

            CapsuleColliderTriger.height = Mathf.Lerp(0, finalHeight, (finalHeightTrigger / time));
            CapsuleColliderTriger.center = new Vector3(0, -(CapsuleColliderTriger.height / 2) + CapsuleColliderTriger.radius, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ResizeColliders();
        IsHeightChangeBlock = false;
        BlockStatusChanged?.Invoke(IsHeightChangeBlock);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger")) return;
        CollisionStatusChanged?.Invoke(Transform, true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Trigger")) return;
        CollisionStatusChanged?.Invoke(Transform, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trigger")) return;
        CollisionStatusChanged?.Invoke(Transform, false);
    }
}