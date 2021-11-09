using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BNG {
    
    /// <summary>
    /// Helper class to interact with physical levers
    /// </summary>
    public class Lever : MonoBehaviour {

        public Rigidbody ParentRigidbody;

        public AudioClip SwitchOnSound;
        public AudioClip SwitchOffSound;

        /// <summary>
        /// Tolerance before considering a switch flipped On or Off
        /// Ex : 1.25 Tolerance means switch can be 98.25% up and considered switched on
        /// </summary>
        [Tooltip("Tolerance before considering a switch flipped On or Off. Ex : 1.25 Tolerance means switch can be 98.25% up and considered switched on.")]
        public float SwitchTolerance = 1.25f;

        /// <summary>
        /// Current position of the lever as expressed as a percentage 1-100
        /// </summary>
        [Tooltip("Current position of the lever as expressed as a percentage 1-100")]
        public float LeverPercentage;

        [Tooltip("If true the lever will lerp towards the Grabber. If false the lever will instantly point to the grabber")]
        public bool UseSmoothLook = true;

        [Tooltip("The speed at which to Lerp towards the Grabber if UseSmoothLook is enabled")]
        public float SmoothLookSpeed = 15f;

        /// <summary>
        /// If false, the lever's rigidbody will be kinematic when not being held. Disable this if you don't want your lever to interact with physics or if you need moving platform support.
        /// </summary>
        [Tooltip("If false, the lever's rigidbody will be kinematic when not being held. Disable this if you don't want your lever to interact with physics or if you need moving platform support.")]
        public bool AllowPhysicsForces = true;

        /// <summary>
        /// If ReturnToCenter true and KinematicWhileInactive true then the lever will smooth look back to center when not being held
        /// </summary>
        [Tooltip("If ReturnToCenter true and KinematicWhileInactive true then the lever will smooth look back to center when not being held")]
        public bool ReturnToCenter = true;

        /// <summary>
        /// How fast to return to center if not being held
        /// </summary>
        [Tooltip("How fast to return to center if not being held")]
        public float ReturnLookSpeed = 5f;

        /// <summary>
        /// If true the lever will look directly at the Grabber and not factor in an initial offset
        /// </summary>
        [Tooltip("If true the lever will look directly at the Grabber and not factor in an initial offset")]
        public bool SnapToGrabber = false;

        /// <summary>
        /// Called when lever was up, but is now in the down position
        /// </summary>
        [Tooltip("Called when lever was up, but is now in the down position")]
        public UnityEvent onLeverDown;

        /// <summary>
        /// Called when lever was down, but is now in the up position
        /// </summary>
        [Tooltip("Called when lever was down, but is now in the up position")]
        public UnityEvent onLeverUp;

        /// <summary>
        /// Called if the lever changes position at all
        /// </summary>
        [Tooltip("Called if the lever changes position at all")]
        public FloatEvent onLeverChange;

        Grabbable grab;
        Rigidbody rb;
        AudioSource audioSource;
        bool switchedOn = true;
        private SwitchStates switchState;
        private enum SwitchStates
        {
            ON = 1,
            MINDDLE = 2,
            OFF = 3
        }

        ConfigurableJoint configJoint;
        HingeJoint hingeJoint;

        void Awake() {
            grab = GetComponent<Grabbable>();
            rb = GetComponent<Rigidbody>();
            hingeJoint = GetComponent<HingeJoint>();
            configJoint = GetComponent<ConfigurableJoint>();

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && (SwitchOnSound != null || SwitchOffSound != null)) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        void Update() {

            // Update Kinematic Status.
            if (rb) {
                rb.isKinematic = AllowPhysicsForces == false && !grab.BeingHeld;
            }

            // Make sure grab offset is reset when not being held
            if(!grab.BeingHeld) {
                initialOffset = Quaternion.identity;
            }

            // Lock our local position and axis in Update to avoid jitter
            transform.localPosition = Vector3.zero;

            // Get the modified angle of of the lever. Use this to get percentage based on Min and Max angles.
            Vector3 currentRotation = transform.localEulerAngles;
            float angle = Mathf.Floor(currentRotation.x);
            angle = (angle > 180) ? angle - 360 : angle;

            // Set percentage of level position
            LeverPercentage = GetAnglePercentage(angle);

            // Lever value changed event
            OnLeverChange(LeverPercentage);

            // Up / Down Events
            if (LeverPercentage >= 80 && (switchState == SwitchStates.OFF || switchState == SwitchStates.MINDDLE)) { // !switchedOn
                OnLeverUp();
            }
            else if (LeverPercentage <= 1 && (switchState == SwitchStates.ON || switchState == SwitchStates.MINDDLE))
            { // switchedOn
                OnLeverDown();
            }
            else if(LeverPercentage > 10 && LeverPercentage < 80)
            {
                switchState = SwitchStates.MINDDLE;
            }
        }

        public virtual float GetAnglePercentage(float currentAngle) {
            if(hingeJoint) {
                return (currentAngle - hingeJoint.limits.min) / (hingeJoint.limits.max - hingeJoint.limits.min) * 100;
            }

            if (configJoint) {
                return currentAngle / configJoint.linearLimit.limit * 100;
            }

            return 0;
        }

        void FixedUpdate() {            
            // Align lever with Grabber
            doLeverLook();
        }

        Quaternion initialOffset = Quaternion.identity;

        void doLeverLook() {
            // Do Lever Look
            if (grab != null && grab.BeingHeld) {
                // Use the grabber as our look target. 
                Transform target = grab.GetPrimaryGrabber().transform;

                // Store original rotation to be used with smooth look
                Quaternion originalRot = transform.rotation;

                // Convert to local position so we can remove the x axis
                Vector3 localTargetPosition = transform.InverseTransformPoint(target.position);

                // Remove local X axis as this would cause the lever to rotate incorrectly
                localTargetPosition.x = 0f;

                // Convert back to world position 
                Vector3 targetPosition = transform.TransformPoint(localTargetPosition);
                transform.LookAt(targetPosition, transform.up);

                // Get the initial hand offset so our Lever doesn't jump to the grabber when we first grab it
                if (initialOffset == Quaternion.identity) {
                    initialOffset = originalRot * Quaternion.Inverse(transform.rotation);
                }

                if(!SnapToGrabber) {
                    transform.rotation = transform.rotation * initialOffset;
                }

                if (UseSmoothLook) {
                    Quaternion newRot = transform.rotation;
                    transform.rotation = originalRot;
                    transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * SmoothLookSpeed);
                }
            }
            else if(grab != null && !grab.BeingHeld) {
                if(ReturnToCenter && AllowPhysicsForces == false) {
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * ReturnLookSpeed);
                }
            }
        }

        // Callback for lever percentage change
        public virtual void OnLeverChange(float percentage) {
            if(onLeverChange != null) {
                onLeverChange.Invoke(percentage);
            }
        }

        /// <summary>
        /// Lever Moved to down position
        /// </summary>
        public virtual void OnLeverDown() {
            
            if (SwitchOffSound != null && !audioSource.isPlaying) {
                audioSource.clip = SwitchOffSound;
                audioSource.Play();
            }

            if(onLeverDown != null) {
                onLeverDown.Invoke();
            }

            //switchedOn = false;
            switchState = SwitchStates.OFF;
        }

        /// <summary>
        /// Lever moved to up position
        /// </summary>
        public virtual void OnLeverUp() {

            if (SwitchOnSound != null && !audioSource.isPlaying) {
                audioSource.clip = SwitchOnSound;
                audioSource.Play();
            }

            // Fire event
            if(onLeverUp != null) {
                onLeverUp.Invoke();
            }

            //switchedOn = true;
            switchState = SwitchStates.ON;
        }
    }
}
