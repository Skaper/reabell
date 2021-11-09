using UnityEngine;

namespace Invector.vCharacterController
{
    using IK;
    using vShooter;
    [vClassHeader("SHOOTER/MELEE INPUT", iconName = "inputIcon")]
    public class vShooterMeleeInput : vMeleeCombatInput,vIShooterIKController,PlayerController.vILockCamera
    {
        #region Shooter Inputs

        [vEditorToolbar("Inputs")]
        [Header("Shooter Inputs")]
        public GenericInput aimInput = new GenericInput("Mouse1", false, "LT", true, "LT", false);
        public GenericInput shotInput = new GenericInput("Mouse0", false, "RT", true, "RT", false);
        public GenericInput secundaryShotInput = new GenericInput("Mouse2", false, "X", true, "X", false);
        public GenericInput reloadInput = new GenericInput("R", "LB", "LB");
        public GenericInput switchCameraSideInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
        public GenericInput scopeViewInput = new GenericInput("Z", "RB", "RB");

        #endregion

        #region Shooter Variables       

        [HideInInspector] public vShooterManager shooterManager;

        internal bool _isAiming;
        internal bool isReloading;
        internal bool defaultStrafeWalk;
        internal Transform leftHand, rightHand, rightLowerArm, leftLowerArm, rightUpperArm, leftUpperArm;
        internal Vector3 aimPosition;
        internal float _aimTimming;
        internal bool ignoreIK = false;
        protected int onlyArmsLayer;
        protected int shootCountA;
        protected int shootCountB;
        protected bool allowAttack;
        protected bool aimConditions;
        protected bool isUsingScopeView;
        protected bool isCameraRightSwitched;
        protected float onlyArmsLayerWeight;
        protected float supportIKWeight, weaponIKWeight;

        protected float armAlignmentWeight;
        protected float aimWeight;

        protected float lastAimDistance;
        protected Quaternion handRotation, upperArmRotation;

        protected vHeadTrack headTrack;

        protected bool lastRotateWithCamera;
        private vControlAimCanvas _controlAimCanvas;
        internal GameObject aimAngleReference;

        private Vector3 ikRotationOffset;
        private Vector3 ikPositionOffset;

        private Quaternion upperArmRotationAlignment, handRotationAlignment;

        #region IKController Interface Properties

        public vIKSolver LeftIK { get; set; }

        public vIKSolver RightIK{ get; set; }

        public vWeaponIKAdjustList WeaponIKAdjustList
        {
            get
            {
                if (shooterManager)
                {
                    return shooterManager.weaponIKAdjustList;
                }


                return null;
            }
            set
            {
                if (shooterManager)
                {
                    shooterManager.weaponIKAdjustList=value;
                }
            }
        }

        public vWeaponIKAdjust CurrentWeaponIK
        {
            get
            {
                if (shooterManager)
                {
                    return shooterManager.CurrentWeaponIK;
                }


                return null;
            }           
        }

        public void SetIKAdjust(vWeaponIKAdjust iKAdjust)
        {
            if (shooterManager) shooterManager.SetIKAdjust(iKAdjust);
        }

        public void LoadIKAdjust(string weaponCategory)
        {
            if(shooterManager)
            {
                shooterManager.LoadIKAdjust(CurrentActiveWeapon.weaponCategory);
            }
        }

        public bool LockAiming
        {
            get
            {
                return shooterManager && shooterManager.alwaysAiming;
            }
            set
            {
                shooterManager.alwaysAiming =value;
            }
        }

        public bool IsCrouching
        {
            get
            {
                return cc.isCrouching;
            }
            set
            {
                cc.isCrouching=value;
            }
        }

        public bool IsLeftWeapon
        {
            get
            {

                return shooterManager && shooterManager.IsLeftWeapon;
            }
        }

        public bool LockCamera
        {
            get
            {
               return tpCamera && tpCamera.lockCamera;
            }
            set
            {
                if(tpCamera)
                tpCamera.lockCamera = value;
            }
        }
        #endregion

        private bool _ignoreIKFromAnimator;

        private bool IsIgnoreIK
        {
            get
            {
                return ignoreIK || _ignoreIKFromAnimator;
            }
        }
        /// <summary>
        /// Is Aiming by input or hipFire
        /// </summary>
        public bool IsAiming
        {
            get
            {
                return _isAiming || _aimTimming > 0;
            }
        }

        public vControlAimCanvas controlAimCanvas
        {
            get
            {
                if (!_controlAimCanvas)
                {
                    _controlAimCanvas = FindObjectOfType<vControlAimCanvas>();
                    if (_controlAimCanvas)
                        _controlAimCanvas.Init(cc);
                }

                return _controlAimCanvas;
            }
        }

        internal bool lockShooterInput;

        public override bool lockInventory
        {
            get
            {
                return base.lockInventory || isReloading;
            }
        }

        #endregion

        protected override void Start()
        {
            shooterManager = GetComponent<vShooterManager>();

            base.Start();

            leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

            onlyArmsLayer = animator.GetLayerIndex("OnlyArms");
            aimAngleReference = new GameObject("aimAngleReference");
            aimAngleReference.tag = ("Ignore Ragdoll");
            aimAngleReference.transform.rotation = transform.rotation;
            var chest = animator.GetBoneTransform(HumanBodyBones.Head);
            aimAngleReference.transform.SetParent(chest);
            aimAngleReference.transform.localPosition = Vector3.zero;
            defaultStrafeWalk = cc.strafeSpeed.walkByDefault;
            headTrack = GetComponent<vHeadTrack>();
            lastRotateWithCamera = cc.strafeSpeed.rotateWithCamera;
            if (!controlAimCanvas)
                Debug.LogWarning("Missing the AimCanvas, drag and drop the prefab to this scene in order to Aim", gameObject);
        }

        protected override void LateUpdate()
        {
            if ((!updateIK && animator.updateMode == AnimatorUpdateMode.AnimatePhysics)) return;
            base.LateUpdate();
            UpdateAimBehaviour();
        }

        #region Shooter Inputs    

        /// <summary>
        /// Lock only shooter inputs
        /// </summary>
        /// <param name="value">lock or unlock</param>
        public virtual void SetLockShooterInput(bool value)
        {
            lockShooterInput = value;

            if (value)
            {
                cc.isStrafing = false;
                isBlocking = false;
                _isAiming = false;
                _aimTimming = 0f;
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
            }
        }

        public override void SetLockAllInput(bool value)
        {
            base.SetLockAllInput(value);
            SetLockShooterInput(value);
        }

        /// <summary>
        /// Set Always Aiming
        /// </summary>
        /// <param name="value">value to set aiming</param>
        public virtual void SetAlwaysAim(bool value)
        {
            shooterManager.alwaysAiming = value;
        }

        /// <summary>
        /// Current active weapon (if weapon gameobject is disabled this return null)
        /// </summary>
        public virtual vShooterWeapon CurrentActiveWeapon
        {
            get
            {
                return shooterManager.CurrentWeapon && shooterManager.IsCurrentWeaponActive() ? shooterManager.CurrentWeapon : null;
            }
        }

        protected override void InputHandle()
        {
            if (cc == null || lockInput || cc.isDead)
                return;

            #region BasicInput

            //if (!isAttacking)
            //{
            if (!cc.ragdolled)
            {
                MoveInput();
                SprintInput();
                CrouchInput();
                StrafeInput();
                JumpInput();
                RollInput();
            }
            //}
            //else
            //    cc.input = Vector2.zero;

            #endregion

            #region MeleeInput

            if (MeleeAttackConditions() && !_isAiming && !isReloading && !lockMeleeInput && (shooterManager.CurrentWeapon == null || (CurrentActiveWeapon == null && !shooterManager.hipfireShot)))
            {
                MeleeWeakAttackInput();
                MeleeStrongAttackInput();
                BlockingInput();
            }
            else
                isBlocking = false;

            #endregion

            #region ShooterInput

            if (lockShooterInput)
            {
                _isAiming = false;
            }
            else
            {
                if (shooterManager == null || CurrentActiveWeapon == null || isEquipping)
                {
                    if (_isAiming)
                    {
                        _isAiming = false;
                        if (cc.isStrafing) cc.Strafe();
                        if (controlAimCanvas != null)
                        {
                            controlAimCanvas.SetActiveAim(false);
                            controlAimCanvas.SetActiveScopeCamera(false);
                        }
                        if (shooterManager && shooterManager.CurrentWeapon && shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0) CurrentActiveWeapon.powerCharge = 0;
                        if (shooterManager && shooterManager.CurrentWeapon && shooterManager.CurrentWeapon.secundaryWeapon != null && shooterManager.CurrentWeapon.secundaryWeapon.chargeWeapon && shooterManager.CurrentWeapon.secundaryWeapon.powerCharge != 0) shooterManager.CurrentWeapon.secundaryWeapon.powerCharge = 0;
                        shootCountA = 0;
                        shootCountB = 0;
                    }
                }
                else
                {
                    AimInput();
                    ShotInput();
                    ReloadInput();
                    SwitchCameraSideInput();
                    ScopeViewInput();
                }
            }

            #endregion
        }

        /// <summary>
        /// Control Aim Input
        /// </summary>
        public virtual void AimInput()
        {
            //Change Rotation Method While Aiming 
            cc.strafeSpeed.rotateWithCamera = IsAiming ? true : lastRotateWithCamera;

            if (!shooterManager || isAttacking)
            {
                _isAiming = false;
                cc.strafeSpeed.walkByDefault = defaultStrafeWalk;
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
                if (cc.isStrafing) cc.Strafe();
                return;
            }

            if (shooterManager.onlyWalkWhenAiming)
                cc.strafeSpeed.walkByDefault = _isAiming ? true : defaultStrafeWalk;

            if (cc.locomotionType == vThirdPersonMotor.LocomotionType.OnlyFree)
            {
                Debug.LogWarning("Shooter behaviour needs to be OnlyStrafe or Free with Strafe. \n Please change the Locomotion Type.");
                return;
            }

            if (shooterManager.hipfireShot)
            {
                if (_aimTimming > 0)
                    _aimTimming -= Time.deltaTime;
            }

            if (!shooterManager || !CurrentActiveWeapon)
            {
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
                _isAiming = false;
                if (cc.isStrafing) cc.Strafe();
                return;
            }

            if (!cc.isRolling)
                _isAiming = !isReloading && (aimInput.GetButton() || (shooterManager.alwaysAiming)) && !cc.ragdolled && !cc.customAction || (cc.customAction && cc.isJumping);

            if (headTrack)
                headTrack.alwaysFollowCamera = _isAiming;


            if (cc.locomotionType == vThirdPersonMotor.LocomotionType.FreeWithStrafe)
            {
                if (IsAiming && !cc.isStrafing)
                {
                    cc.Strafe();
                }
                else if (!IsAiming && cc.isStrafing)
                {
                    cc.Strafe();
                }
            }
            if (IsAiming && shooterManager.onlyWalkWhenAiming && cc.isSprinting) cc.isSprinting = false;

            if (controlAimCanvas)
            {
                if (IsAiming && !controlAimCanvas.isAimActive)
                    controlAimCanvas.SetActiveAim(true);
                if (!IsAiming && controlAimCanvas.isAimActive)
                    controlAimCanvas.SetActiveAim(false);
            }
            if (shooterManager.rWeapon)
            {
                shooterManager.rWeapon.SetActiveAim(IsAiming && aimConditions);
                shooterManager.rWeapon.SetActiveScope(IsAiming && isUsingScopeView);
            }
            else if (shooterManager.lWeapon)
            {
                shooterManager.lWeapon.SetActiveAim(IsAiming && aimConditions);
                shooterManager.lWeapon.SetActiveScope(IsAiming && isUsingScopeView);
            }
        }

        /// <summary>
        /// Control shot inputs (primary and secundary weapons)
        /// </summary>
        public virtual void ShotInput()
        {
            if (!shooterManager || CurrentActiveWeapon == null || cc.isDead)
            {
                if (shooterManager && shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0) CurrentActiveWeapon.powerCharge = 0;
                if (shooterManager && shooterManager.CurrentWeapon.secundaryWeapon != null && shooterManager.CurrentWeapon.secundaryWeapon.chargeWeapon && shooterManager.CurrentWeapon.secundaryWeapon.powerCharge != 0) shooterManager.CurrentWeapon.secundaryWeapon.powerCharge = 0;
                shootCountA = 0;
                shootCountB = 0;
                return;
            }

            if ((IsAiming && !shooterManager.hipfireShot || shooterManager.hipfireShot) && !shooterManager.isShooting && aimConditions && !isReloading && !isAttacking)
            {
                if (CurrentActiveWeapon || (shooterManager.CurrentWeapon && shooterManager.hipfireShot))
                {
                    HandleShotCount(shooterManager.CurrentWeapon, false, shotInput.GetButton());
                }
                if ((CurrentActiveWeapon || (shooterManager.CurrentWeapon && shooterManager.hipfireShot)) && shooterManager.CurrentWeapon.secundaryWeapon)
                {
                    HandleShotCount(shooterManager.CurrentWeapon.secundaryWeapon, true, secundaryShotInput.GetButton());
                }
            }
            else if (!IsAiming)
            {
                if (shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0) CurrentActiveWeapon.powerCharge = 0;
                if (shooterManager.CurrentWeapon.secundaryWeapon != null && shooterManager.CurrentWeapon.secundaryWeapon.chargeWeapon && shooterManager.CurrentWeapon.secundaryWeapon.powerCharge != 0) shooterManager.CurrentWeapon.secundaryWeapon.powerCharge = 0;
                shootCountA = 0;
                shootCountB = 0;
            }
            shooterManager.UpdateShotTime();
        }

        /// <summary>
        /// Control Shot count
        /// </summary>
        /// <param name="weapon">target weapon</param>
        /// <param name="secundaryShot">target weapon is secundary?</param>
        /// <param name="weaponInput">check input</param>
        public virtual void HandleShotCount(vShooterWeapon weapon, bool secundaryShot = false, bool weaponInput = true)
        {
            if (weapon.chargeWeapon)
            {
                if (shooterManager.WeaponHasLoadedAmmo(secundaryShot) && weapon.powerCharge < 1 && weaponInput)
                {
                    if (shooterManager.hipfireShot) _aimTimming = shooterManager.HipfireAimTime;
                    weapon.powerCharge += Time.deltaTime * weapon.chargeSpeed;
                }
                else if ((weapon.powerCharge >= 1 && weapon.autoShotOnFinishCharge && weaponInput) || (!weaponInput && (_isAiming || (shooterManager.hipfireShot && _aimTimming > 0)) && weapon.powerCharge > 0))
                {
                    if (shooterManager.hipfireShot) _aimTimming = shooterManager.HipfireAimTime;
                    if (secundaryShot)
                        shootCountB++;
                    else
                        shootCountA++;
                    weapon.powerCharge = 0;
                }
                animator.SetFloat(vAnimatorParameters.PowerCharger, weapon.powerCharge);
            }
            else if (weapon.automaticWeapon && weaponInput)
            {
                if (shooterManager.hipfireShot) _aimTimming = shooterManager.HipfireAimTime;
                if (secundaryShot)
                    shootCountB++;
                else
                    shootCountA++;
            }
            else if (weaponInput)
            {
                if (allowAttack == false)
                {
                    if (shooterManager.hipfireShot) _aimTimming = shooterManager.HipfireAimTime;
                    if (secundaryShot)
                        shootCountB++;
                    else
                        shootCountA++;
                    allowAttack = true;
                }
            }
            else allowAttack = false;
        }

        /// <summary>
        /// Do Shots by shotcount (primary and secundary weapons) after Ik behaviour updated
        /// </summary>
        public virtual void DoShots()
        {
            if (shootCountA > 0)
            {
                if (CanDoShots())
                {
                    shootCountA--;
                    shooterManager.Shoot(aimPosition, !_isAiming, false);
                }
            }
            if (shootCountB > 0)
            {
                if (CanDoShots())
                {
                    shootCountB--;
                    shooterManager.Shoot(aimPosition, !_isAiming, true);
                }
            }
        }

        /// <summary>
        /// Shot's the primary shot
        /// </summary>
        /// <param name="inputValue">It simulates a click - True to shot, false to reset, true to shot again</param>
        public virtual void ShotPrimary(bool inputValue = true)
        {
            if ((_isAiming && !shooterManager.hipfireShot || shooterManager.hipfireShot) && !shooterManager.isShooting && aimConditions && !isReloading && !isAttacking)
            {
                if (CurrentActiveWeapon) HandleShotCount(CurrentActiveWeapon, false, inputValue);
            }
        }

        /// <summary>
        /// Shot's the secundary shot
        /// </summary>
        /// <param name="inputValue">True to charge, false to shot</param>
        public virtual void ShotSecundary(bool inputValue = true)
        {
            if ((_isAiming && !shooterManager.hipfireShot || shooterManager.hipfireShot) && !shooterManager.isShooting && aimConditions && !isReloading && !isAttacking)
            {
                if (CurrentActiveWeapon && CurrentActiveWeapon.secundaryWeapon)
                    HandleShotCount(CurrentActiveWeapon.secundaryWeapon, true, inputValue);
            }
        }

        /// <summary>
        /// Reload current weapon
        /// </summary>
        public virtual void ReloadInput()
        {
            if (!shooterManager || CurrentActiveWeapon == null) return;
            if (reloadInput.GetButtonDown() && !isReloading && !cc.customAction && !cc.ragdolled)
            {
                _aimTimming = 0f;
                shooterManager.ReloadWeapon();
            }
        }

        /// <summary>
        /// Control Switch Camera side Input
        /// </summary>
        public virtual void SwitchCameraSideInput()
        {
            if (tpCamera == null) return;
            if (switchCameraSideInput.GetButtonDown())
            {
                SwitchCameraSide();
            }
        }

        /// <summary>
        /// Change side view of the <seealso cref="Invector.vCamera.vThirdPersonCamera"/>
        /// </summary>
        public virtual void SwitchCameraSide()
        {
            if (tpCamera == null) return;
            isCameraRightSwitched = !isCameraRightSwitched;
            tpCamera.SwitchRight(isCameraRightSwitched);
        }

        /// <summary>
        /// Reset the Aiming and AimCanvas to false
        /// </summary>
        public void CancelAiming()
        {
            _isAiming = false;
            _aimTimming = 0;
            if (controlAimCanvas)
            {
                controlAimCanvas.SetActiveAim(false);
                controlAimCanvas.SetActiveScopeCamera(false);
            }
            if (cc.isStrafing) cc.Strafe();
        }

        /// <summary>
        /// Control Scope view input
        /// </summary>
        public virtual void ScopeViewInput()
        {
            if (!shooterManager || CurrentActiveWeapon == null) return;

            if (_isAiming && aimConditions && (scopeViewInput.GetButtonDown() || CurrentActiveWeapon.onlyUseScopeUIView))
            {
                if (controlAimCanvas && CurrentActiveWeapon.scopeTarget)
                {
                    if (!isUsingScopeView && CurrentActiveWeapon.onlyUseScopeUIView) EnableScopeView();
                    else if (isUsingScopeView && !CurrentActiveWeapon.onlyUseScopeUIView) DisableScopeView();
                    else if (!isUsingScopeView) EnableScopeView();
                }
            }
            else if (isUsingScopeView && (controlAimCanvas && !_isAiming || controlAimCanvas && !aimConditions || cc.isRolling))
            {
                DisableScopeView();
            }
        }

        /// <summary>
        /// Enable scope view (just if is aiming)
        /// </summary>
        public virtual void EnableScopeView()
        {
            if (!_isAiming) return;
            isUsingScopeView = true;
            controlAimCanvas.SetActiveScopeCamera(true, CurrentActiveWeapon.useUI);
        }

        /// <summary>
        /// Disable scope view
        /// </summary>
        public virtual void DisableScopeView()
        {
            isUsingScopeView = false;
            controlAimCanvas.SetActiveScopeCamera(false);
        }

        /// <summary>
        /// Enable the BlockInput if you don't have any Shooter Weapons equipped
        /// </summary>
        public override void BlockingInput()
        {
            if (shooterManager == null || CurrentActiveWeapon == null)
                base.BlockingInput();
        }

        #endregion

        #region Update Animations

        protected override void UpdateMeleeAnimations()
        {
            // disable the onlyarms layer and run the melee methods if the character is not using any shooter weapon
            if (!animator) return;

            if (cc.customAction)
            {
                ResetMeleeAnimations();
                ResetShooterAnimations();
                // reset to the default camera state
                UpdateCameraStates();
                // reset the aiming
                CancelAiming();
                return;
            }
            // update MeleeManager Animator Properties
            if ((shooterManager == null || !CurrentActiveWeapon) && meleeManager)
            {
                base.UpdateMeleeAnimations();
                // set the uppbody id (armsonly layer)
                //animator.SetFloat(vAnimatorParameters.UpperBody_ID, 0, .2f, Time.deltaTime);
                // turn on the onlyarms layer to aim 
                onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0, 6f * Time.deltaTime);
                animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
                // reset aiming parameter
                animator.SetBool(vAnimatorParameters.IsAiming, false);
                isReloading = false;
            }
            // update ShooterManager Animator Properties
            else if (shooterManager && CurrentActiveWeapon)
                UpdateShooterAnimations();
            // reset Animator Properties
            else
            {
                ResetMeleeAnimations();
                ResetShooterAnimations();
            }
        }

        public virtual void ResetShooterAnimations()
        {
            if (shooterManager == null || !animator) return;
            // set the uppbody id (armsonly layer)
            animator.SetFloat(vAnimatorParameters.UpperBody_ID, 0, .2f, Time.deltaTime);
            // set if the character can aim or not (upperbody layer)
            animator.SetBool(vAnimatorParameters.CanAim, false);
            // character is aiming
            animator.SetBool(vAnimatorParameters.IsAiming, false);
            // turn on the onlyarms layer to aim 
            onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0, 6f * Time.deltaTime);
            animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
        }

        protected virtual void UpdateShooterAnimations()
        {
            if (shooterManager == null) return;

            if ((!_isAiming && _aimTimming <= 0) && meleeManager)
            {
                // set attack id from the melee weapon (trigger fullbody atk animations)
                animator.SetInteger(vAnimatorParameters.AttackID, meleeManager.GetAttackID());
            }
            else
            {
                // set attack id from the shooter weapon (trigger shot layer animations)
                animator.SetFloat(vAnimatorParameters.Shot_ID, shooterManager.GetShotID());
            }
            // turn on the onlyarms layer to aim 
            onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, (CurrentActiveWeapon) ? 1f : 0f, 6f * Time.deltaTime);
            animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);

            if (CurrentActiveWeapon != null && !shooterManager.useDefaultMovesetWhenNotAiming || (_isAiming || _aimTimming > 0))
            {
                // set the move set id (base layer) 
                animator.SetFloat(vAnimatorParameters.MoveSet_ID, shooterManager.GetMoveSetID(), .1f, Time.deltaTime);
            }
            else if (shooterManager.useDefaultMovesetWhenNotAiming)
            {
                // set the move set id (base layer) 
                animator.SetFloat(vAnimatorParameters.MoveSet_ID, 0, .1f, Time.deltaTime);
            }
            // set the isBlocking false while using shooter weapons
            animator.SetBool(vAnimatorParameters.IsBlocking, false);
            // set the uppbody id (armsonly layer)
            animator.SetFloat(vAnimatorParameters.UpperBody_ID, shooterManager.GetUpperBodyID(), .2f, Time.deltaTime);
            // set if the character can aim or not (upperbody layer)
            animator.SetBool(vAnimatorParameters.CanAim, aimConditions);
            // character is aiming
            animator.SetBool(vAnimatorParameters.IsAiming, (_isAiming || _aimTimming > 0) && !isAttacking);
            // find states with the Reload tag
            isReloading = cc.IsAnimatorTag("IsReloading") || shooterManager.isReloadingWeapon;
            // find states with the IsEquipping tag
            isEquipping = cc.IsAnimatorTag("IsEquipping");
            // Check if Animator state need to ignore IK
            _ignoreIKFromAnimator = cc.IsAnimatorTag("IgnoreIK");
        }

        public override void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vCamera.vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }

            if (changeCameraState)
                tpCamera.ChangeState(customCameraState, customlookAtPoint, true);
            else if (cc.isCrouching)
                tpCamera.ChangeState("Crouch", true);
            else if (cc.isStrafing && !_isAiming)
                tpCamera.ChangeState("Strafing", true);
            else if (_isAiming && CurrentActiveWeapon)
            {
                if (string.IsNullOrEmpty(CurrentActiveWeapon.customAimCameraState))
                    tpCamera.ChangeState("Aiming", true);
                else
                    tpCamera.ChangeState(CurrentActiveWeapon.customAimCameraState, true);
            }
            else
                tpCamera.ChangeState("Default", true);
        }

        #endregion

        #region Update Aim

        protected virtual void UpdateAimPosition()
        {
            if (!shooterManager) return;

            if (CurrentActiveWeapon == null) return;

            var camT = isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeCamera ? //Check if is using canvas scope view
                    CurrentActiveWeapon.zoomScopeCamera ? /* if true, check if weapon has a zoomScopeCamera, 
                if true...*/
                    CurrentActiveWeapon.zoomScopeCamera.transform : controlAimCanvas.scopeCamera.transform :
                    /*else*/cameraMain.transform;

            var origin1 = camT.position;
            if (!(controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera))
                origin1 = camT.position;

            var vOrigin = origin1;
            vOrigin += controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? camT.forward : Vector3.zero;
            aimPosition = camT.position + camT.forward * 100f;
            //aimAngleReference.transform.eulerAngles = new Vector3(aimAngleReference.transform.eulerAngles.x, transform.eulerAngles.y, aimAngleReference.transform.eulerAngles.z);
            if (!isUsingScopeView) lastAimDistance = 100f;

            if (shooterManager.raycastAimTarget && CurrentActiveWeapon.raycastAimTarget)
            {
                RaycastHit hit;
                Ray ray = new Ray(vOrigin, camT.forward);

                if (Physics.Raycast(ray, out hit, cameraMain.farClipPlane, shooterManager.damageLayer))
                {
                    if (hit.collider.transform.IsChildOf(transform))
                    {
                        var collider = hit.collider;
                        var hits = Physics.RaycastAll(ray, cameraMain.farClipPlane, shooterManager.damageLayer);
                        var dist = cameraMain.farClipPlane;
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].distance < dist && hits[i].collider.gameObject != collider.gameObject && !hits[i].collider.transform.IsChildOf(transform))
                            {
                                dist = hits[i].distance;
                                hit = hits[i];
                            }
                        }
                    }

                    if (hit.collider)
                    {
                        if (!isUsingScopeView)
                            lastAimDistance = Vector3.Distance(camT.position, hit.point);
                        aimPosition = hit.point;
                    }
                }
                if (shooterManager.showCheckAimGizmos)
                {
                    Debug.DrawLine(ray.origin, aimPosition);
                }
            }
            if (_isAiming)
                shooterManager.CameraSway();
        }

        #endregion

        #region IK behaviour

        void OnDrawGizmos()
        {
            if (!shooterManager || !shooterManager.showCheckAimGizmos) return;
            var weaponSide = isCameraRightSwitched ? -1 : 1;
            var _ray = new Ray(aimAngleReference.transform.position + transform.up * shooterManager.blockAimOffsetY + transform.right * shooterManager.blockAimOffsetX * weaponSide, cameraMain.transform.forward);
            Gizmos.DrawRay(_ray.origin, _ray.direction * shooterManager.minDistanceToAim);
            var color = Gizmos.color;
            color = aimConditions ? Color.green : Color.red;
            color.a = 1f;
            Gizmos.color = color;
            Gizmos.DrawSphere(_ray.GetPoint(shooterManager.minDistanceToAim), shooterManager.checkAimRadius);
            Gizmos.DrawSphere(aimPosition, shooterManager.checkAimRadius);
        }

        protected virtual void UpdateAimBehaviour()
        {
            UpdateAimPosition();
            UpdateHeadTrack();
            if (shooterManager && CurrentActiveWeapon)
            {
                UpdateIKAdjust(shooterManager.IsLeftWeapon);
                RotateAimArm(shooterManager.IsLeftWeapon);
                RotateAimHand(shooterManager.IsLeftWeapon);
                UpdateArmsIK(shooterManager.IsLeftWeapon);
            }
            if (isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeCamera) UpdateAimPosition();
            CheckAimConditions();
            UpdateAimHud();
            DoShots();
           
        }

        protected virtual void UpdateIKAdjust(bool isUsingLeftHand)
        {
            vWeaponIKAdjust weaponIKAdjust = shooterManager.CurrentWeaponIK;
            if (!weaponIKAdjust || IsIgnoreIK)
            {
                weaponIKWeight = 0;
                return;
            }
            weaponIKWeight = Mathf.Lerp(weaponIKWeight, cc.customAction || isReloading || isEquipping ? 0 : 1, 25f * Time.deltaTime);
            if (weaponIKWeight <= 0) return;
            // create left arm ik solver if equal null
            if (LeftIK == null || !LeftIK.isValidBones)
            {
                LeftIK = new vIKSolver(animator, AvatarIKGoal.LeftHand);
                LeftIK.UpdateIK();
            }
            if (RightIK == null || !RightIK.isValidBones)
            {
                RightIK = new vIKSolver(animator, AvatarIKGoal.RightHand);
                RightIK.UpdateIK();
            }
            if (isUsingLeftHand)
            {
                ApplyOffsets(weaponIKAdjust, LeftIK, RightIK);
            }
            else
            {
                ApplyOffsets(weaponIKAdjust, RightIK, LeftIK);
            }
        }

        protected virtual void ApplyOffsets(vWeaponIKAdjust weaponIKAdjust, vIKSolver weaponHand, vIKSolver supportHand)
        {
            bool isValid = weaponIKAdjust != null;
            weaponHand.SetIKWeight(weaponIKWeight);
            IKAdjust ikAdjust = isValid ? weaponIKAdjust.GetIKAdjust(_isAiming || _aimTimming > 0, cc.isCrouching) : null;
            //Apply Offset to Weapon Arm
            ApplyOffsetToTargetBone(isValid ? ikAdjust.weaponHandOffset : null, weaponHand.endBoneOffset, isValid);
            ApplyOffsetToTargetBone(isValid ? ikAdjust.weaponHintOffset : null, weaponHand.middleBoneOffset, isValid);
            //Apply offset to Support Weapon Arm
            ApplyOffsetToTargetBone(isValid ? ikAdjust.supportHandOffset : null, supportHand.endBoneOffset, isValid);
            ApplyOffsetToTargetBone(isValid ? ikAdjust.supportHintOffset : null, supportHand.middleBoneOffset, isValid);
            //Convert Animatorion To IK with offsets applied
            if (isValid) weaponHand.AnimationToIK();
        }

        protected virtual void ApplyOffsetToTargetBone(IKOffsetTransform iKOffset, Transform target, bool isValid)
        {
            target.localPosition = Vector3.Lerp(target.localPosition, isValid ? iKOffset.position : Vector3.zero, 10f * Time.deltaTime);
            target.localRotation = Quaternion.Lerp(target.localRotation, isValid ? Quaternion.Euler(iKOffset.eulerAngles) : Quaternion.Euler(Vector3.zero), 10f * Time.deltaTime);
        }

        protected virtual void UpdateArmsIK(bool isUsingLeftHand = false)
        {
            if (!shooterManager || !CurrentActiveWeapon || !shooterManager.useLeftIK) return;
            if (cc.IsAnimatorTag("Shot Fire") && CurrentActiveWeapon.disableIkOnShot || IsIgnoreIK) { supportIKWeight = 0; return; }

            bool useIkConditions = false;
            var animatorInput = cc.input.magnitude;
            if (!_isAiming && !isAttacking)
            {
                if (animatorInput < 1f)
                    useIkConditions = CurrentActiveWeapon.useIkOnIdle;
                else if (cc.isStrafing)
                    useIkConditions = CurrentActiveWeapon.useIkOnStrafe;
                else
                    useIkConditions = CurrentActiveWeapon.useIkOnFree;
            }
            else if (_isAiming && !isAttacking) useIkConditions = CurrentActiveWeapon.useIKOnAiming;
            else if (isAttacking) useIkConditions = CurrentActiveWeapon.useIkAttacking;

            // create left arm ik solver if equal null
            if (LeftIK == null || !LeftIK.isValidBones) LeftIK = new vIKSolver(animator, AvatarIKGoal.LeftHand);
            if (RightIK == null || !RightIK.isValidBones) RightIK = new vIKSolver(animator, AvatarIKGoal.RightHand);
            vIKSolver targetIK = null;

            if (isUsingLeftHand)
                targetIK = RightIK;
            else
                targetIK = LeftIK;

            if (targetIK != null)
            {
                if (shooterManager.weaponIKAdjustList)
                {
                    if (isUsingLeftHand)
                    {
                        ikRotationOffset = shooterManager.weaponIKAdjustList.ikRotationOffsetR;
                        ikPositionOffset = shooterManager.weaponIKAdjustList.ikPositionOffsetR;
                    }
                    else
                    {
                        ikRotationOffset = shooterManager.weaponIKAdjustList.ikRotationOffsetL;
                        ikPositionOffset = shooterManager.weaponIKAdjustList.ikPositionOffsetL;
                    }
                }
                // control weight of ik
                if (CurrentActiveWeapon && CurrentActiveWeapon.handIKTarget && Time.timeScale > 0 && !isReloading && !cc.customAction && !isEquipping && (cc.isGrounded || (_isAiming || _aimTimming > 0f)) && !cc.lockAnimMovement && useIkConditions)
                    supportIKWeight = Mathf.Lerp(supportIKWeight, 1, 10f * Time.deltaTime);
                else
                    supportIKWeight = Mathf.Lerp(supportIKWeight, 0, 25f * Time.deltaTime);

                if (supportIKWeight <= 0) return;
                // update IK
                targetIK.SetIKWeight(supportIKWeight);
                if (shooterManager && CurrentActiveWeapon && CurrentActiveWeapon.handIKTarget)
                {
                    var _offset = (CurrentActiveWeapon.handIKTarget.forward * ikPositionOffset.z) + (CurrentActiveWeapon.handIKTarget.right * ikPositionOffset.x) + (CurrentActiveWeapon.handIKTarget.up * ikPositionOffset.y);
                    targetIK.SetIKPosition(CurrentActiveWeapon.handIKTarget.position + _offset);
                    var _rotation = Quaternion.Euler(ikRotationOffset);
                    targetIK.SetIKRotation(CurrentActiveWeapon.handIKTarget.rotation * _rotation);
                    if (shooterManager.CurrentWeaponIK)
                        targetIK.AnimationToIK();
                }
            }
        }

        protected virtual bool CanRotateAimArm()
        {
            return  cc.IsAnimatorTag("Upperbody Pose") && IsAimAlignWithForward();
        }

        protected virtual bool CanDoShots()
        {
            return  armAlignmentWeight >= 0.01f && cc.IsAnimatorTag("Upperbody Pose") && cc.upperBodyInfo.normalizedTime>0.5f;
        }

        protected virtual void RotateAimArm(bool isUsingLeftHand = false)
        {
            if (!shooterManager) return;

            armAlignmentWeight = IsAiming && aimConditions && CanRotateAimArm() ? Mathf.Lerp(armAlignmentWeight, shooterManager.isShooting ? 0 : (1f * (Mathf.Clamp(cc.upperBodyInfo.normalizedTime, 0, 1f))), shooterManager.smoothArmAlignWeight * (Time.deltaTime)) : 0;
            if (CurrentActiveWeapon && armAlignmentWeight > 0.01f && CurrentActiveWeapon.alignRightUpperArmToAim)
            {
                var aimPoint = targetArmAlignmentPosition;
                Vector3 v = aimPoint - CurrentActiveWeapon.aimReference.position;
                var orientation = CurrentActiveWeapon.aimReference.forward;

                var upperArm = isUsingLeftHand ? leftUpperArm : rightUpperArm;
                var rot = Quaternion.FromToRotation(upperArm.InverseTransformDirection(orientation), upperArm.InverseTransformDirection(v));

                if ((!float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z)))
                    upperArmRotationAlignment = rot;

                var angle = Vector3.Angle(aimPosition - aimAngleReference.transform.position, aimAngleReference.transform.forward);

                if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || controlAimCanvas && controlAimCanvas.isScopeCameraActive)
                {
                    upperArmRotation = Quaternion.Lerp(upperArmRotation, upperArmRotationAlignment, shooterManager.smoothArmIKRotation * (.001f + Time.deltaTime));
                }

                if (!float.IsNaN(upperArmRotation.x) && !float.IsNaN(upperArmRotation.y) && !float.IsNaN(upperArmRotation.z))
                {
                    var armWeight = CurrentActiveWeapon.alignRightHandToAim ? Mathf.Clamp(armAlignmentWeight, 0, 0.5f) : armAlignmentWeight;
                    upperArm.localRotation *= Quaternion.Euler(upperArmRotation.eulerAngles.NormalizeAngle() * armWeight);
                }

            }
            else
            {
                upperArmRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        protected virtual void RotateAimHand(bool isUsingLeftHand = false)
        {
            if (!shooterManager) return;

            if (CurrentActiveWeapon && armAlignmentWeight > 0.01f && aimConditions && CurrentActiveWeapon.alignRightHandToAim)
            {
                var aimPoint = targetArmAlignmentPosition;
                Vector3 v = aimPoint - CurrentActiveWeapon.aimReference.position;
                var orientation = CurrentActiveWeapon.aimReference.forward;
                var hand = isUsingLeftHand ? leftHand : rightHand;
                var rot = Quaternion.FromToRotation(hand.InverseTransformDirection(orientation), hand.InverseTransformDirection(v));
                if ((!float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z)))
                    handRotationAlignment = rot;
                var angle = Vector3.Angle(aimPosition - aimAngleReference.transform.position, aimAngleReference.transform.forward);
                if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || (controlAimCanvas && controlAimCanvas.isScopeCameraActive))
                    handRotation = Quaternion.Lerp(handRotation, handRotationAlignment, shooterManager.smoothArmIKRotation * (.001f + Time.deltaTime));

                if (!float.IsNaN(handRotation.x) && !float.IsNaN(handRotation.y) && !float.IsNaN(handRotation.z))
                {
                    var armWeight = CurrentActiveWeapon.alignRightUpperArmToAim ? Mathf.Clamp(armAlignmentWeight, 0, 0.5f) : armAlignmentWeight;
                    hand.localRotation *= Quaternion.Euler(handRotation.eulerAngles.NormalizeAngle() * armWeight);
                }


                CurrentActiveWeapon.SetScopeLookTarget(aimPoint);
            }
            else handRotation = Quaternion.Euler(0, 0, 0);
        }

        #region Old Rotate Arm system
        //protected virtual void RotateAimArm(bool isUsingLeftHand = false)
        //{
        //    if (!shooterManager) return;

        //    if (CurrentActiveWeapon && (isAiming || aimTimming > 0f) && aimConditions && CurrentActiveWeapon.alignRightUpperArmToAim)
        //    {
        //        var aimPoint = targetArmAlignmentPosition;
        //        Vector3 v = aimPoint - CurrentActiveWeapon.aimReference.position;
        //        Vector3 v2 = Quaternion.AngleAxis(-CurrentActiveWeapon.recoilUp, CurrentActiveWeapon.aimReference.right) * v;
        //        var orientation = CurrentActiveWeapon.aimReference.forward;
        //        armAlignmentWeight = Mathf.Lerp(armAlignmentWeight, !shooterManager.isShooting || CurrentActiveWeapon.ammoCount <= 0 ? 1f * aimWeight : 0f, 1f * Time.deltaTime);
        //        var upperArm = isUsingLeftHand ? leftUpperArm : rightUpperArm;
        //        var r = Quaternion.FromToRotation(orientation, v) * upperArm.rotation;
        //        var r2 = Quaternion.FromToRotation(orientation, v2) * upperArm.rotation;
        //        Quaternion rot = Quaternion.Lerp(r2, r, armAlignmentWeight);
        //        var angle = Vector3.Angle(aimPosition - aimAngleReference.transform.position, aimAngleReference.transform.forward);

        //        if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || controlAimCanvas && controlAimCanvas.isScopeCameraActive)
        //        {
        //            upperArmRotation = Quaternion.Lerp(upperArmRotation, rot, shooterManager.smoothArmIKRotation * Time.deltaTime);
        //        }
        //        else upperArmRotation = upperArm.rotation;

        //        if (!float.IsNaN(upperArmRotation.x) && !float.IsNaN(upperArmRotation.y) && !float.IsNaN(upperArmRotation.z))
        //            upperArm.rotation = upperArmRotation;
        //    }
        //}

        //protected virtual void RotateAimHand(bool isUsingLeftHand = false)
        //{
        //    if (!shooterManager) return;

        //    if (CurrentActiveWeapon && CurrentActiveWeapon.alignRightHandToAim && (isAiming || aimTimming > 0f) && aimConditions)
        //    {
        //        var aimPoint = targetArmAlignmentPosition;
        //        Vector3 v = aimPoint - CurrentActiveWeapon.aimReference.position;
        //        Vector3 v2 = Quaternion.AngleAxis(-CurrentActiveWeapon.recoilUp, CurrentActiveWeapon.aimReference.right) * v;
        //        var orientation = CurrentActiveWeapon.aimReference.forward;

        //        if (!CurrentActiveWeapon.alignRightUpperArmToAim)
        //            armAlignmentWeight = Mathf.Lerp(armAlignmentWeight, !shooterManager.isShooting || CurrentActiveWeapon.ammoCount <= 0 ? 1f * aimWeight : 0f, 1f * Time.deltaTime);

        //        var hand = isUsingLeftHand ? leftHand : rightHand;
        //        var r = Quaternion.FromToRotation(orientation, v) * hand.rotation;
        //        var r2 = Quaternion.FromToRotation(orientation, v2) * hand.rotation;
        //        Quaternion rot = Quaternion.Lerp(r2, r, armAlignmentWeight);
        //        var angle = Vector3.Angle(aimPosition - aimAngleReference.transform.position, aimAngleReference.transform.forward);

        //        if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || (controlAimCanvas && controlAimCanvas.isScopeCameraActive))
        //            handRotation = Quaternion.Lerp(handRotation, rot, shooterManager.smoothArmIKRotation * Time.deltaTime);
        //        else handRotation = Quaternion.Lerp(hand.rotation, rot, shooterManager.smoothArmIKRotation * Time.deltaTime);

        //        if (!float.IsNaN(handRotation.x) && !float.IsNaN(handRotation.y) && !float.IsNaN(handRotation.z))
        //            hand.rotation = handRotation;

        //        CurrentActiveWeapon.SetScopeLookTarget(aimPoint);
        //    }
        //}
        #endregion

        protected virtual void CheckAimConditions()
        {
            if (!shooterManager) return;
            var weaponSide = isCameraRightSwitched ? -1 : 1;

            if (CurrentActiveWeapon == null)
            {
                aimConditions = false;
                return;
            }
            if (!shooterManager.hipfireShot && !IsAimAlignWithForward())
            {
                aimConditions = false;
            }
            else
            {
                var _ray = new Ray(aimAngleReference.transform.position + transform.up * shooterManager.blockAimOffsetY + transform.right * shooterManager.blockAimOffsetX * weaponSide, cameraMain.transform.forward);
                RaycastHit hit;
                if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, out hit, shooterManager.minDistanceToAim, shooterManager.blockAimLayer))
                {
                    aimConditions = false;
                }
                else
                    aimConditions = true;
            }

            aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 10 * Time.deltaTime);
        }

        protected virtual bool IsAimAlignWithForward()
        {
            if (!shooterManager) return false;
            var dir = targetArmAligmentDirection;
            dir.Normalize();
            dir.y = 0;
            var angle = Quaternion.LookRotation(dir.normalized, Vector3.up).eulerAngles - transform.eulerAngles;

            return ((angle.NormalizeAngle().y < 15 && angle.NormalizeAngle().y > -15));
        }

        protected virtual Vector3 targetArmAlignmentPosition
        {
            get
            {
                return isUsingScopeView && controlAimCanvas.scopeCamera ? cameraMain.transform.position + cameraMain.transform.forward * lastAimDistance : aimPosition;
            }
        }

        protected virtual Vector3 targetArmAligmentDirection
        {
            get
            {
                var t = controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeCamera ? controlAimCanvas.scopeCamera.transform : cameraMain.transform;
                return t.forward;
            }
        }

        protected virtual void UpdateHeadTrack()
        {
            if (headTrack) headTrack.ignoreSmooth = IsAiming;
            if (!shooterManager || !headTrack)
            {
                if (headTrack)
                {
                    headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.Smooth);
                    headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, Vector2.zero, headTrack.Smooth);
                    
                }
                return;
            }
            if (!CurrentActiveWeapon || !headTrack || !shooterManager.CurrentWeaponIK)
            {
                if (headTrack)
                {
                    headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.Smooth);
                    headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, Vector2.zero, headTrack.Smooth);
                }
                return;
            }
            if (_isAiming || _aimTimming > 0f)
            {
                var offsetSpine = cc.isCrouching ? shooterManager.CurrentWeaponIK.crouchingAiming.spineOffset.spine : shooterManager.CurrentWeaponIK.standingAiming.spineOffset.spine;
                var offsetHead = cc.isCrouching ? shooterManager.CurrentWeaponIK.crouchingAiming.spineOffset.head : shooterManager.CurrentWeaponIK.standingAiming.spineOffset.head;
                headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, offsetSpine, headTrack.Smooth );
                headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, offsetHead, headTrack.Smooth);
            }
            else
            {
                var offsetSpine = cc.isCrouching ? shooterManager.CurrentWeaponIK.crouching.spineOffset.spine : shooterManager.CurrentWeaponIK.standing.spineOffset.spine;
                var offsetHead = cc.isCrouching ? shooterManager.CurrentWeaponIK.crouching.spineOffset.head : shooterManager.CurrentWeaponIK.standing.spineOffset.head;
                headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, offsetSpine, headTrack.Smooth );
                headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, offsetHead, headTrack.Smooth);
            }
        }

        protected virtual void UpdateAimHud()
        {
            if (!shooterManager || !controlAimCanvas) return;
            if (CurrentActiveWeapon == null) return;
            controlAimCanvas.SetAimCanvasID(CurrentActiveWeapon.scopeID);
            if (controlAimCanvas.scopeCamera && controlAimCanvas.scopeCamera.gameObject.activeSelf)
                controlAimCanvas.SetAimToCenter(true);
            else if (_isAiming)
            {
                RaycastHit hit;
                if (Physics.Linecast(CurrentActiveWeapon.muzzle.position, aimPosition, out hit, shooterManager.blockAimLayer))
                    controlAimCanvas.SetWordPosition(hit.point,  aimConditions);
                else
                    controlAimCanvas.SetWordPosition(aimPosition, aimConditions);
            }
            else
                controlAimCanvas.SetAimToCenter(true);

            if (CurrentActiveWeapon.scopeTarget)
            {
                var lookPoint = cameraMain.transform.position + (cameraMain.transform.forward * (isUsingScopeView ? lastAimDistance : 100f));
                controlAimCanvas.UpdateScopeCamera(CurrentActiveWeapon.scopeTarget.position, lookPoint, CurrentActiveWeapon.zoomScopeCamera ? 0 : CurrentActiveWeapon.scopeZoom);
            }
        }

        #endregion

    }

    public static partial class vAnimatorParameters
    {
        public static int UpperBody_ID = Animator.StringToHash("UpperBody_ID");
        public static int CanAim = Animator.StringToHash("CanAim");
        public static int IsAiming = Animator.StringToHash("IsAiming");
        public static int Shot_ID = Animator.StringToHash("Shot_ID");
        public static int PowerCharger = Animator.StringToHash("PowerCharger");
    }
}