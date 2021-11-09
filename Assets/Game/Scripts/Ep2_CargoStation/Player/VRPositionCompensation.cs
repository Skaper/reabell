using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPositionCompensation : MonoBehaviour
{
    public Transform mainObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 avatarPos = transform.position;
        Vector3 mainPos = mainObject.position;
        if (!Vector3.Equals(avatarPos, mainPos))
        {
            mainObject.position = avatarPos;
            transform.localPosition = Vector3.zero;
        }
    }
}
