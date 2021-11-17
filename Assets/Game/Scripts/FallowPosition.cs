using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallowPosition : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    public bool lockYPosition;
    public float yPosition;
    private void Awake()
    {
        offset = GameManager.PlayerReferences.CharacterOffset;
    }
    void Update()
    {
        transform.localPosition = new Vector3(target.localPosition.x, yPosition, target.localPosition.z);
    }

}
