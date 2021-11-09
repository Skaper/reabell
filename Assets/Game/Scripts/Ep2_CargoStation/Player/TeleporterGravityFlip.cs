using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterGravityFlip : MonoBehaviour
{
    public float targetAngle = 0f;

    public Quaternion getGravityRotation()
    {
        return Quaternion.Euler(new Vector3(targetAngle, 0f, 0f));
    }
}
