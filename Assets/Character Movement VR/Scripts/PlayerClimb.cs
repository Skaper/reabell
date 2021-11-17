using UnityEngine;
using CMF;

[RequireComponent(typeof(PlayerReferences))]
public class PlayerClimb : MonoBehaviour
{
    public BodyColliderBehaviour GravityColliderScript;

    private Rigidbody m_Rigidbody;
    private AdvancedWalkerController m_walker;

    private PlayerInput_Interactor m_ClimbInfluencer;
    private Vector3 m_ClimbXRRigReference;
    private Vector3 m_CollisionCorrection;

    private Vector3 m_BodyCollisionPosition;
    private Vector3 m_LastKnownGoodPosition;
    private bool m_Colliding;
    private float m_CorrectionRecoveryRate = 0.01f;

    public bool Climbing
    {
        get
        {
            return (m_ClimbInfluencer != null);
        }
    }

    void Awake()
    {
        m_Rigidbody = GetComponent<PlayerReferences>().RigRigidbody;
        m_walker = GetComponent<PlayerReferences>().PlayerWalkerController;
        m_ClimbInfluencer = null;
        m_CollisionCorrection = Vector3.zero;
        GravityColliderScript.BlockStatusChanged += OnBodyColliderHeightBlock;
    }

    private void OnBodyColliderHeightBlock(bool isBlock)
    {
        m_Rigidbody.isKinematic = isBlock;
    }

    public void AddInfluencer(PlayerInput_Interactor NewInfluencer)
    {
        if (m_ClimbInfluencer == NewInfluencer)
            return;

        if (m_ClimbInfluencer == null)
            GravityColliderScript.CollisionStatusChanged += HandleCollision;
        m_walker.DisableGravity();
        m_ClimbInfluencer = NewInfluencer;
        m_Rigidbody.velocity = Vector3.zero;

        ResetReferences();
    }

    public void RemoveInfluencer(PlayerInput_Interactor InfluencerToRemove)
    {
        if (m_ClimbInfluencer != InfluencerToRemove)
            return; 
        m_ClimbInfluencer = null;
        m_walker.EnableGravity();
        GravityColliderScript.CollisionStatusChanged -= HandleCollision;
        m_CollisionCorrection = Vector3.zero;
        m_Colliding = false;
        m_Rigidbody.AddForce(-3f * InfluencerToRemove.velocities.Velocity, ForceMode.VelocityChange); // fling
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!m_Colliding)
        {
            m_LastKnownGoodPosition = m_Rigidbody.position;
        }

        if (m_ClimbInfluencer != null)
        {
            GravityColliderScript.BlockHeightChange();
            Vector3 TargetPositionDelta = m_ClimbInfluencer.XRRigTargetDelta;

            if (m_Colliding)
            {
                m_CollisionCorrection += m_LastKnownGoodPosition - m_BodyCollisionPosition;
                Debug.Log("m_Colliding");
            }
            else if (m_CollisionCorrection != Vector3.zero)
            {
                m_CollisionCorrection -= m_CollisionCorrection.normalized * Mathf.Min(m_CorrectionRecoveryRate, m_CollisionCorrection.magnitude);
            }

            m_Rigidbody.MovePosition(
                m_ClimbXRRigReference
                + m_CollisionCorrection
                + TargetPositionDelta
            );
        }
        else
        {
            GravityColliderScript.UnblockHeightChange();
        }
    }

    void HandleCollision(Transform transform, bool colliding)
    {
        m_Colliding = colliding;

        if (m_Colliding)
            m_BodyCollisionPosition = m_Rigidbody.position;
    }

    void ResetReferences()
    {
        m_ClimbXRRigReference = m_Rigidbody.position;

        m_ClimbInfluencer?.ResetReference();
    }
}
