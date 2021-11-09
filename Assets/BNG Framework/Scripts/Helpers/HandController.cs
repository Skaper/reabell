﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// An example hand controller that sets animation values depending on Grabber state
    /// </summary>
    public class HandController : MonoBehaviour {

        [Tooltip("HandController parent will be set to this on Start if specified")]
        public Transform HandAnchor;

        public bool ResetHandAnchorPosition = true;

        public Animator HandAnimator;

        [Tooltip("Check the state of this grabber to determine animation state. If null, a child Grabber component will be used.")]
        public Grabber grabber;

        /// <summary>
        /// 0 = Open Hand, 1 = Full Grip
        /// </summary>
        public float GripAmount;
        private float _prevGrip;

        /// <summary>
        /// 0 = Index Curled in,  1 = Pointing Finger
        /// </summary>
        public float PointAmount;
        private float _prevPoint;

        /// <summary>
        /// 0 = Thumb Down, 1 = Thumbs Up
        /// </summary>
        public float ThumbAmount;
        private float _prevThumb;

        public int PoseId;

        /// <summary>
        /// How fast to Lerp the Layer Animations
        /// </summary>
        public float HandAnimationSpeed = 20f;

        InputBridge input;

        void Start() {

            if(HandAnchor) {
                transform.parent = HandAnchor;

                if(ResetHandAnchorPosition) {
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
            
            if(grabber == null) {
                grabber = GetComponentInChildren<Grabber>();
            }
            
            input = InputBridge.Instance;
        }

        void Update() {

            // Grabber may have been deactivated
            if (grabber == null || !grabber.isActiveAndEnabled) {
                grabber = GetComponentInChildren<Grabber>();
                GripAmount = 0;
                PointAmount = 0;
                ThumbAmount = 0;
                return;
            }

            if (grabber.HandSide == ControllerHand.Left) {
                GripAmount = input.LeftGrip;
                PointAmount = 1 - input.LeftTrigger; // Range between 0 and 1. 1 == Finger all the way out
                PointAmount *= InputBridge.Instance.InputSource == XRInputSource.SteamVR ? 0.25F : 0.5F; // Reduce the amount our finger points out if Oculus or XRInput

                // If not near the trigger, point finger all the way out
                if (input.SupportsIndexTouch && input.LeftTriggerNear == false && PointAmount != 0) {
                    PointAmount = 1f;
                }

                ThumbAmount = input.LeftThumbNear ? 0 : 1;
            }
            else if (grabber.HandSide == ControllerHand.Right) {
                GripAmount = input.RightGrip;
                PointAmount = 1 - input.RightTrigger; // Range between 0 and 1. 1 == Finger all the way out
                PointAmount *= InputBridge.Instance.InputSource == XRInputSource.SteamVR ? 0.25F : 0.5F; // Reduce the amount our finger points out if Oculus or XRInput

                // If not near the trigger, point finger all the way out
                if (input.SupportsIndexTouch && input.RightTriggerNear == false && PointAmount != 0) {
                    PointAmount = 1f;
                }

                ThumbAmount = input.RightThumbNear ? 0 : 1;
            }            

            // Try getting child animator
            if(HandAnimator == null || !HandAnimator.isActiveAndEnabled) {
                HandAnimator = GetComponentInChildren<Animator>();
            }

            if (HandAnimator != null) {
                updateAnimimationStates();
            }
        }

        void updateAnimimationStates()
        {            
            if(HandAnimator != null && HandAnimator.isActiveAndEnabled && HandAnimator.runtimeAnimatorController != null) {

                _prevGrip = Mathf.Lerp(_prevGrip, GripAmount, Time.deltaTime * HandAnimationSpeed);
                _prevThumb = Mathf.Lerp(_prevThumb, ThumbAmount, Time.deltaTime * HandAnimationSpeed);
                _prevPoint = Mathf.Lerp(_prevPoint, PointAmount, Time.deltaTime * HandAnimationSpeed);

                // 0 = Hands Open, 1 = Grip closes                        
                HandAnimator.SetFloat("Flex", _prevGrip);

                HandAnimator.SetLayerWeight(1, _prevThumb);

                //// 0 = pointer finger inwards, 1 = pointing out    
                //// Point is played as a blend
                //// Near trigger? Push finger down a bit
                HandAnimator.SetLayerWeight(2, _prevPoint);

                // Should we use a custom hand pose?
                if (grabber.HeldGrabbable != null) {
                    HandAnimator.SetLayerWeight(0, 0);
                    HandAnimator.SetLayerWeight(1, 0);
                    HandAnimator.SetLayerWeight(2, 0);

                    PoseId = (int)grabber.HeldGrabbable.CustomHandPose;

                    if (grabber.HeldGrabbable.ActiveGrabPoint != null) {

                        // Get the Min / Max of our finger blends if set by the user
                        // This allows a pose to blend between states
                        // Index Finger
                        setAnimatorBlend(grabber.HeldGrabbable.ActiveGrabPoint.IndexBlendMin, grabber.HeldGrabbable.ActiveGrabPoint.IndexBlendMax, PointAmount, 2);

                        // Thumb
                        setAnimatorBlend(grabber.HeldGrabbable.ActiveGrabPoint.ThumbBlendMin, grabber.HeldGrabbable.ActiveGrabPoint.ThumbBlendMax, ThumbAmount, 1);                       
                    }
                    else {
                        // Force everything to grab if we're holding something
                        if (grabber.HoldingItem) {
                            GripAmount = 1;
                            PointAmount = 0;
                            ThumbAmount = 0;
                        }
                    }

                    HandAnimator.SetInteger("Pose", PoseId);
                }
                else {
                    HandAnimator.SetInteger("Pose", 0);
                }
            }

            void setAnimatorBlend(float min, float max, float input, int animationLayer) {
                HandAnimator.SetLayerWeight(animationLayer, min + (input) * max - min);
            }
        }
    }
}