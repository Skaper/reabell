using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public enum LocomotionType {
        Teleport,
        SmoothLocomotion,
        None
    }

    /// <summary>
    /// The BNGPlayerController handles basic player movement and climbing.
    /// </summary>
    public class BNGPlayerController : MonoBehaviour {

        [Header("Locomotion : ")]


        [SerializeField]
        LocomotionType selectedLocomotion = LocomotionType.Teleport;
        public LocomotionType SelectedLocomotion {
            get { return selectedLocomotion; }
        }

        [Header("Camera Options : ")]

        [Tooltip("If true the CharacterController will move along with the HMD, as long as there are no obstacle's in the way")]
        public bool MoveCharacterWithCamera = true;

        [Tooltip("If true the CharacterController will rotate it's Y angle to match the HMD's Y angle")]
        public bool RotateCharacterWithCamera = true;

        [Header("Transform Setup ")]

        [Tooltip("The TrackingSpace represents your tracking space origin.")]
        public Transform TrackingSpace;

        [Tooltip("The CameraRig is a Transform that is used to offset the main camera. The main camera should be parented to this.")]
        public Transform CameraRig;

        [Tooltip("The CenterEyeAnchor is typically the Transform that contains your Main Camera")]
        public Transform CenterEyeAnchor;


        [Header("Gravity : ")]
        /// <summary>
        /// Is Gravity Currently Enabled for this Character
        /// </summary>
        public bool GravityEnabled = true;
        public float GravityAmount = 7.1f;

        [Header("Ground checks : ")]
        [Tooltip("Raycast against these layers to check if player is grounded")]
        public LayerMask GroundedLayers;

        private Vector3 moveDirection = Vector3.zero;

        /// <summary>
        /// 0 means we are grounded
        /// </summary>
        [HideInInspector]
        public float DistanceFromGround = 0;

        [Header("Player Capsule Settings : ")]
        /// <summary>
        /// Minimum Height our Player's capsule collider can be (in meters)
        /// </summary>
        [Tooltip("Minimum Height our Player's capsule collider can be (in meters)")]
        public float MinimumCapsuleHeight = 0.4f;

        /// <summary>
        /// Maximum Height our Player's capsule collider can be (in meters)
        /// </summary>
        [Tooltip("Maximum Height our Player's capsule collider can be (in meters)")]
        public float MaximumCapsuleHeight = 3f;        

        [Tooltip("Set the player's capsule collider height to this amount while climbing. This can allow you to shorten the capsule collider a bit, making it easier to navigate over ledges.")]
        public float ClimbingCapsuleHeight = 0.5f;

        [Tooltip("Set the player's capsule collider capsule center to this amount while climbing.")]
        public float ClimbingCapsuleCenter = -0.25f;

        [HideInInspector]
        public float LastTeleportTime;

        [Header("Player Y Offset : ")]
        /// <summary>
        /// Offset the height of the CharacterController by this amount
        /// </summary>
        [Tooltip("Offset the height of the CharacterController by this amount")]
        public float CharacterControllerYOffset = -0.025f;

        /// <summary>
        /// Height of our camera in local coords
        /// </summary>
        [HideInInspector]
        public float CameraHeight;

        [Header("Misc : ")]                

        [Tooltip("If true the Camera will be offset by ElevateCameraHeight if no HMD is active or connected. This prevents the camera from falling to the floor and can allow you to use keyboard controls.")]
        public bool ElevateCameraIfNoHMDPresent = true;

        [Tooltip("How high (in meters) to elevate the player camera if no HMD is present and ElevateCameraIfNoHMDPresent is true. 1.65 = about 5.4' tall. ")]
        public float ElevateCameraHeight = 1.65f;

        /// <summary>
        /// If player goes below this elevation they will be reset to their initial starting position.
        /// If the player goes too far away from the center they may start to jitter due to floating point precisions.
        /// Can also use this to detect if player somehow fell through a floor. Or if the "floor is lava".
        /// </summary>
        [Tooltip("Minimum Y position our player is allowed to go. Useful for floating point precision and making sure player didn't fall through the map.")]
        public float MinElevation = -6000f;

        /// <summary>
        /// If player goes above this elevation they will be reset to their initial starting position.
        /// If the player goes too far away from the center they may start to jitter due to floating point precisions.
        /// </summary>
        public float MaxElevation = 6000f;

        [Header("Shown for Debug : ")]

        /// <summary>
        /// Whether or not we are currently holding on to something climbable with 1 or more grabbers
        /// </summary>
        public bool GrippingClimbable = false;


        [HideInInspector]
        public float LastPlayerMoveTime;

        // Any climber grabbers in use
        List<Grabber> climbers;

        // The controller to manipulate
        CharacterController characterController;

        // Optional components can be used to update LastMoved Time
        PlayerTeleport teleport;
        SmoothLocomotion smoothLocomotion;
        PlayerRotation playerRotation;

        // This the object that is currently beneath us
        RaycastHit groundHit;
        Transform mainCamera;
        Vector3 lastPlayerPosition;
        Quaternion lastPlayerRotation;
        float lastSnapTime;

        private float _initialGravityModifier;
        private Vector3 _initialPosition;
        private Transform _initialCharacterParent;

        void Start() {
            characterController = GetComponentInChildren<CharacterController>();
            mainCamera = Camera.main.transform;

            if (characterController) {
                _initialCharacterParent = characterController.transform.parent;
            }

            _initialGravityModifier = GravityAmount;

            _initialPosition = characterController.transform.position;
            float initialY = _initialPosition.y;
            if (initialY < MinElevation) {
                Debug.LogWarning("Initial Starting Position is lower than Minimum Elevation. Increasing Min Elevation to " + MinElevation);
                MinElevation = initialY;
            }
            if (initialY > MaxElevation) {
                Debug.LogWarning("Initial Starting Position is greater than Maximum Elevation. Reducing Max Elevation to " + MaxElevation);
                MaxElevation = initialY;
            }

            teleport = GetComponent<PlayerTeleport>();
            smoothLocomotion = GetComponentInChildren<SmoothLocomotion>();
            playerRotation = GetComponentInChildren<PlayerRotation>();

            climbers = new List<Grabber>();

            // Player root must be at 0,0,0 for Tracking Space to work properly.
            // If this player transform was moved in the editor on load, we can fix it by moving the CharacterController to the position
            if (transform.position != Vector3.zero || transform.localEulerAngles != Vector3.zero) {
                Vector3 playerPos = transform.position;
                Quaternion playerRot = transform.rotation;

                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;

                if (characterController) {
                    characterController.transform.position = playerPos;
                    characterController.transform.rotation = playerRot;
                }

                Debug.Log("Player position not set to 0. Moving player to : " + playerPos);
            }

            ChangeLocomotionType(selectedLocomotion);
        }

        void Update() {

            // Sanity check for camera
            if (mainCamera == null) {
                mainCamera = Camera.main.transform;
            }            
            
            // Update the Character Controller's Capsule Height to match our Camera position
            UpdateCharacterHeight();

            // Update the position of our camera rig to account for our player's height
            UpdateCameraRigPosition();

            // After positioning the camera rig, we can update our main camera's height
            UpdateCameraHeight();

            UpdateDistanceFromGround();

            CheckCharacterCollisionMove();

            if (characterController) {

                // Align TrackingSpace with Camera
                if (RotateCharacterWithCamera) {
                    RotateTrackingSpaceToCamera();
                }

                // Update Last snap time based on character controller rotation
                if (Mathf.Abs(Quaternion.Angle(lastPlayerRotation, characterController.transform.rotation)) > 1) {
                    UpdateLastSnapTime();
                }
            }

            checkMovingPlatform();

            checkClimbing();

            // Store player position so we can compare against it next frame
            lastPlayerPosition = characterController.transform.position;
            lastPlayerRotation = characterController.transform.rotation;
        }

        void FixedUpdate() {
            // Player should never go above or below 6000 units as physics can start to jitter due to floating point precision
            if (characterController && characterController.transform.position.y < MinElevation || characterController.transform.position.y > MaxElevation) {
                Debug.Log("Player out of bounds; Returning to initial position.");
                characterController.transform.position = _initialPosition;
            }
        }

        public virtual void UpdateDistanceFromGround() {
            if (Physics.Raycast(characterController.transform.position, -characterController.transform.up, out groundHit, 20, GroundedLayers, QueryTriggerInteraction.Ignore)) {
                DistanceFromGround = Vector3.Distance(characterController.transform.position, groundHit.point);
                DistanceFromGround += characterController.center.y;
                DistanceFromGround -= (characterController.height * 0.5f) + characterController.skinWidth;

                // Round to nearest thousandth
                DistanceFromGround = (float)Math.Round(DistanceFromGround * 1000f) / 1000f;
            }
            else {
                DistanceFromGround = 9999f;
            }
        }

        public virtual void RotateTrackingSpaceToCamera() {
            Vector3 initialPosition = TrackingSpace.position;
            Quaternion initialRotation = TrackingSpace.rotation;

            // Move the character controller to the proper rotation / alignment
            characterController.transform.rotation = Quaternion.Euler(0.0f, CenterEyeAnchor.rotation.eulerAngles.y, 0.0f);

            // Now we can rotate our tracking space back to initial position / rotation
            TrackingSpace.position = initialPosition;
            TrackingSpace.rotation = initialRotation;
        }

        // Update the last time we snapped the player rotation
        public virtual void UpdateLastSnapTime() {
            lastSnapTime = Time.time; 
        }

        public virtual bool RecentlyMoved() {

            // Recently Moved if position changed to teleport of some kind
            if(Vector3.Distance(lastPlayerPosition, characterController.transform.position) > 0.001f) {
                return true;
            }

            // Considered recently moved if just teleported
            if (Time.time - LastTeleportTime < 0.1f) {
                return true;
            }

            // Considered recently moved if just moved using PlayerController (for example, snap turning)
            if (Time.time - LastPlayerMoveTime < 0.1f) {
                return true;
            }

            // Recently Moved if position changed to teleport of some kind
            if (Vector3.Distance(lastPlayerPosition, characterController.transform.position) > 0.001f) {
                return true;
            }

            // Recently Snap Turned through rotation
            if(playerRotation != null && playerRotation.RecentlySnapTurned()) {
                return true;
            }

            // Recent Snap Turn
            if(Time.time - lastSnapTime < 0.2f) {
                return true;
            }

            return false;
        }        

        public virtual void UpdateCameraRigPosition() {
            float yPos = -(0.5f * characterController.height) + characterController.center.y + CharacterControllerYOffset;
            if (grippingAtLeastOneClimbable()) {
                yPos -= 0.25f;
            }

            // If no HMD is active, bump our rig up a bit so it doesn't sit on the floor
            if(!InputBridge.Instance.HMDActive && ElevateCameraIfNoHMDPresent) {
                yPos += ElevateCameraHeight;
            }

            CameraRig.transform.localPosition = new Vector3(CameraRig.transform.localPosition.x, yPos, CameraRig.transform.localPosition.z);
        }

        public virtual void UpdateCharacterHeight() {
            float minHeight = MinimumCapsuleHeight;
            // Increase Min Height if no HMD is present. This prevents our character from being really small
            if(!InputBridge.Instance.HMDActive && minHeight < 1f) {
                minHeight = 1f;
            }

            // Update Character Height based on Camera Height.
            characterController.height = Mathf.Clamp(CameraHeight + CharacterControllerYOffset - characterController.skinWidth, minHeight, MaximumCapsuleHeight);

            // If we are climbing set the capsule center upwards
            if (grippingAtLeastOneClimbable()) {
                characterController.height = ClimbingCapsuleHeight;
                characterController.center = new Vector3(0, ClimbingCapsuleCenter, 0);
            }
            else {
                characterController.center = new Vector3(0, -0.25f, 0);
            }
        }

        public virtual void UpdateCameraHeight() {
            // update camera height
            if (CenterEyeAnchor) {
                CameraHeight = CenterEyeAnchor.localPosition.y;
            }
        }


        public virtual void CheckCharacterCollisionMove() {

            if(!MoveCharacterWithCamera) {
                return;
            }
            
            Vector3 initialCameraRigPosition = CameraRig.transform.position;
            Vector3 cameraPosition = CenterEyeAnchor.position;
            Vector3 delta = cameraPosition - characterController.transform.position;
            
            // Ignore Y position
            delta.y = 0;

            // Move Character Controller and Camera Rig to Camera's delta
            if (delta.magnitude > 0.0f) {
                characterController.Move(delta);

                // Move Camera Rig back into position
                CameraRig.transform.position = initialCameraRigPosition;
            }
        }

        bool grippingAtLeastOneClimbable() {

            if(climbers != null && climbers.Count > 0) {

                for(int x = 0; x < climbers.Count; x++) {
                    // Climbable is still being held
                    if(climbers[x] != null && climbers[x].HoldingItem) {
                        return true;
                    }
                }

                // If we made it through every climber and none were valid, reset the climbers
                climbers = new List<Grabber>();
            }

            return false;
        }

        void checkClimbing() {
            GrippingClimbable = grippingAtLeastOneClimbable();

            if (GrippingClimbable) {

                if(smoothLocomotion) {
                    smoothLocomotion.DisableMovement();
                }

                moveDirection = Vector3.zero;

                int count = 0;
                for (int i = 0; i < climbers.Count; i++) {
                    Grabber climber = climbers[i];
                    if (climber != null && climber.HoldingItem) {
                        Vector3 climberMoveAmount = climber.PreviousPosition - climber.transform.position;

                        if (count == 0) {
                            moveDirection += climberMoveAmount;
                        }
                        else {
                            moveDirection += climberMoveAmount - moveDirection;
                        }

                        count++;
                    }
                }

                characterController.Move(moveDirection);
            }
            else {
                if (smoothLocomotion) {
                    smoothLocomotion.EnableMovement();
                }
            }

            // Update any climber previous position
            for (int x = 0; x < climbers.Count; x++) {
                Grabber climber = climbers[x];
                if (climber != null && climber.HoldingItem) {
                    if (climber.DummyTransform != null) {
                        // Use climber position if possible
                        climber.PreviousPosition = climber.DummyTransform.position;
                    }
                    else {
                        climber.PreviousPosition = climber.transform.position;
                    }
                }
            }
        }

        public virtual void checkMovingPlatform() {
            bool onMovingPlatform = false;

            Vector3 moveDir = Vector3.zero;

            if (groundHit.collider != null && DistanceFromGround < 0.01f) {
                MoveToWaypoint waypoint = groundHit.collider.gameObject.GetComponent<MoveToWaypoint>();
                MovingPlatform platform = groundHit.collider.gameObject.GetComponent<MovingPlatform>();

                if (platform) {
                    onMovingPlatform = true;

                    if (waypoint.PositionDifference != Vector3.zero) {
                        // This is another potential method of moving the character instead of parenting it
                        //characterController.Move(platform.PositionDifference);
                    }
                }
            }

            // For now we can parent the objet to move it along
            if (onMovingPlatform) {
                characterController.transform.parent = groundHit.collider.transform;
            }
            else {
                characterController.transform.parent = _initialCharacterParent;
            }
        }

        public void ChangeLocomotionType(LocomotionType loc) {
            selectedLocomotion = loc;

            if(teleport == null) {
                teleport = GetComponent<PlayerTeleport>();
            }

            toggleTeleport(selectedLocomotion == LocomotionType.Teleport);
            toggleSmoothLocomotion(selectedLocomotion == LocomotionType.SmoothLocomotion);
        }

        void toggleTeleport(bool enabled) {
            if(enabled) {
                teleport.EnableTeleportation();
            }
            else {
                teleport.DisableTeleportation();
            }
        }

        void toggleSmoothLocomotion(bool enabled) {
            if(smoothLocomotion) {
                smoothLocomotion.enabled = enabled;
            }
        }

        public void ToggleLocomotionType() {
            // Toggle based on last value
            if(selectedLocomotion == LocomotionType.SmoothLocomotion) {
                ChangeLocomotionType(LocomotionType.Teleport);
            }
            else {
                ChangeLocomotionType(LocomotionType.SmoothLocomotion);
            }
        }

        public bool IsGrounded() {

            // Immediately check for a positive from a CharacterController if it's present
            if(characterController != null) {
                if(characterController.isGrounded) {
                    return true;
                }
            }

            // DistanceFromGround is a bit more reliable as we can give a bit of leniency in what's considered grounded
            return DistanceFromGround <= 0.001f;
        }

        public void ToggleGravity(bool gravityOn) {

            GravityEnabled = gravityOn;


            if (gravityOn) {
                GravityAmount = _initialGravityModifier;
            }
            else {
                GravityAmount = 0;
            }
        }

        public void AddClimber(Climbable climbable, Grabber grab) {
            if (!climbers.Contains(grab)) {

                if(grab.DummyTransform == null) {
                    GameObject go = new GameObject();
                    go.transform.name = "DummyTransform";
                    go.transform.parent = grab.transform;
                    go.transform.position = grab.transform.position;
                    go.transform.localEulerAngles = Vector3.zero;

                    grab.DummyTransform = go.transform;
                }

                // Set parent to whatever we grabbed. This way we can follow the object around if it moves
                grab.DummyTransform.parent = climbable.transform;
                grab.PreviousPosition = grab.DummyTransform.position;

                climbers.Add(grab);
            }
        }

        public void RemoveClimber(Grabber grab) {
            if (climbers.Contains(grab)) {
                // Reset grabbable parent
                grab.DummyTransform.parent = grab.transform;
                grab.DummyTransform.localPosition = Vector3.zero;

                climbers.Remove(grab);
            }
        }
    }
}
