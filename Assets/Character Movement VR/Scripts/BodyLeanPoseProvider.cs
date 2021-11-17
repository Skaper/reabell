using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;
using UnityEngine.SpatialTracking;
using UnityEngine.Experimental.XR.Interaction;

public class BodyLeanPoseProvider : BasePoseProvider
{
    [Tooltip("Distance in the horizontal plane that the head can move before this body position will follow.")]
    public float followDistance= 0.5f;
    public PlayerReferences playerRef;
    private PlayerClimb m_PlayerClimb;
    //private PlayerWalk m_PlayerWalk;
    private AdvancedWalkerController m_PlayerWalk;
    Pose LastPose;

    private void Awake()
    {
        
        m_PlayerClimb = playerRef.PlayerClimb;
        m_PlayerWalk = playerRef.PlayerWalkerController;
    }

    private void Start()
    {
        LastPose = Pose.identity;
    }

    public override PoseDataFlags GetPoseFromProvider(out Pose output)
    {
        Pose HeadPose;


        if (PoseDataSource.GetDataFromSource(TrackedPoseDriver.TrackedPose.Head, out HeadPose) == PoseDataFlags.NoData)
        {
            output = LastPose;
            return PoseDataFlags.NoData;
        }
        Vector3 rot = HeadPose.rotation.eulerAngles;
        //rot = new Vector3(rot.x, 0f, rot.z);
        //output.rotation = HeadPose.rotation;
        output.rotation = Quaternion.identity;//Quaternion.Euler(rot);
        float Distance = Vector2.Distance(new Vector2(HeadPose.position.x, HeadPose.position.z), new Vector2(LastPose.position.x, LastPose.position.z));
        // climbing - snap to head pos
        if (m_PlayerClimb.Climbing)
            output.position = HeadPose.position;
        // beyond distance - correct it
        else
        {
            Vector3 LastPoseNewHeadHeightPosition = new Vector3(LastPose.position.x, HeadPose.position.y, LastPose.position.z);
            Vector3 LastPoseFollowPositionAdjustment = (followDistance * (LastPoseNewHeadHeightPosition - HeadPose.position).normalized);
            Vector3 MoveTowardsCenterAdjustment = (HeadPose.position - LastPoseNewHeadHeightPosition).normalized * 0.005f;


            if (Distance > followDistance && !m_PlayerWalk.moving)
            {
                output.position = HeadPose.position + LastPoseFollowPositionAdjustment;
            }
            else if (Distance > followDistance && m_PlayerWalk.moving)
                output.position = HeadPose.position + LastPoseFollowPositionAdjustment + MoveTowardsCenterAdjustment;
            else if (m_PlayerWalk.moving)
                output.position = LastPoseNewHeadHeightPosition + MoveTowardsCenterAdjustment;
            else
            {
                output.position = LastPoseNewHeadHeightPosition;
            }
            
        }

        

        LastPose = output;
        return PoseDataFlags.Position | PoseDataFlags.Rotation;
    }
}
