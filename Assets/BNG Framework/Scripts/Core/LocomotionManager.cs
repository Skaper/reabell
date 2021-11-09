using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class LocomotionManager : MonoBehaviour {

        [Header("Locomotion Type")]
        /// <summary>
        /// Default locomotion to use if nothing stored in playerprefs. 0 = Teleport. 1 = SmoothLocomotion
        /// </summary>
        [Tooltip("Default locomotion to use if nothing stored in playerprefs. 0 = Teleport. 1 = SmoothLocomotion")]
        public LocomotionType DefaultLocomotion = LocomotionType.Teleport;

        /// <summary>
        /// If true, locomotion type will be saved and loaded from player prefs
        /// </summary>
        [Header("Save / Loading")]
        [Tooltip("If true, locomotion type will be saved and loaded from player prefs")]
        public bool LoadLocomotionFromPrefs = true;

        [Header("Input")]
        [Tooltip("The key(s) to use to toggle locomotion type")]
        public List<ControllerBinding> locomotionToggleInput = new List<ControllerBinding>() { ControllerBinding.None };

        /// <summary>
        /// (Oculus Only) - Click Left Thumbstick Down to toggle between Teleport / Smooth Locomotion
        /// </summary>
        [Tooltip("(Oculus Only) - Click Left Thumbstick Down to toggle between Teleport / Smooth Locomotion")]
        public bool LeftThumbstickToggleLocomotionOculus = true;

        BNGPlayerController player;
        PlayerTeleport teleport;

        void Start() {
            player = GetComponent<BNGPlayerController>();
            teleport = GetComponent<PlayerTeleport>();

            // Load Locomotion Preference
            if (LoadLocomotionFromPrefs) {
                ChangeLocomotion(PlayerPrefs.GetInt("LocomotionSelection", 0) == 0 ? LocomotionType.Teleport : LocomotionType.SmoothLocomotion, false);
            }
            else {
                ChangeLocomotion(DefaultLocomotion, false);
            }
        }

        void Update() {
            // Check for standard input
            if (CheckLocomotionTypeInput()) {
                ChangeLocomotion(player.SelectedLocomotion == LocomotionType.SmoothLocomotion ? LocomotionType.Teleport : LocomotionType.SmoothLocomotion, LoadLocomotionFromPrefs);
            }
            // Oculus Device Only - Toggle Locomotion by pressing left thumbstick down            
            else if (LeftThumbstickToggleLocomotionOculus && InputBridge.Instance.LeftThumbstickDown && InputBridge.Instance.IsOculusDevice) {
                ChangeLocomotion(player.SelectedLocomotion == LocomotionType.SmoothLocomotion ? LocomotionType.Teleport : LocomotionType.SmoothLocomotion, LoadLocomotionFromPrefs);
            }
        }

        public void UpdateTeleportStatus() {
            teleport.enabled = player.SelectedLocomotion == LocomotionType.Teleport;
        }

        public void ChangeLocomotion(LocomotionType locomotionType, bool save) {
            player.ChangeLocomotionType(locomotionType);

            if (save) {
                PlayerPrefs.SetInt("LocomotionSelection", locomotionType == LocomotionType.Teleport ? 0 : 1);
            }

            UpdateTeleportStatus();
        }

        public bool CheckLocomotionTypeInput() {
            // Check for bound controller button
            for (int x = 0; x < locomotionToggleInput.Count; x++) {
                if (InputBridge.Instance.GetControllerBindingValue(locomotionToggleInput[x])) {
                    return true;
                }
            }

            return false;
        }
    }
}