using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR;

public class XRRigFloorOffset : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {

        TrySetRoomScale();
    }

    public void TrySetRoomScale()
    {
        if (XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale))
        {
            transform.localPosition = Vector3.zero;
            Debug.Log("SUCCESS: XRRigFloorOffset.TrySetRoomScale() on Application.platform= " + Application.platform + " and XRSettings.loadedDeviceName=" + XRSettings.loadedDeviceName);
        }
        else
        {
            transform.localPosition = GameManager.PlayerReferences.characterOffset;
            Debug.Log("FAILURE: XRRigFloorOffset.TrySetRoomScale() on Application.platform= " + Application.platform + " and XRSettings.loadedDeviceName=" + XRSettings.loadedDeviceName);
        }
    }
}
