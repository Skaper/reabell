using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vShooter
{
    using vCharacterController;
    using vItemManager;

    [vClassHeader("SHOOTER MANAGER", iconName = "shooterIcon")]
    public class vShooterManager : vMonoBehaviour
    {
        #region variables

        [System.Serializable]
        public class OnReloadWeapon : UnityEngine.Events.UnityEvent<vShooterWeapon> { }
        public delegate void TotalAmmoHandler(int ammoID, ref int ammo);

        [vEditorToolbar("Damage Layers")]
        [Tooltip("Layer to aim and apply damage")]
        public LayerMask damageLayer = 1 << 0;
        [Tooltip("Tags to ignore (auto add this gameObject tag to avoid damage your self)")]
        public vTagMask ignoreTags = new vTagMask("Player");
        [Tooltip("Layer to block aim")]
        public LayerMask blockAimLayer = 1 << 0;
        public float blockAimOffsetY = 0.35f;
        public float blockAimOffsetX = 0.35f;

        [vEditorToolbar("Cancel Reload")]
        [vHelpBox("You can call the <b>CancelReload</b> method using events to interupt the reload routine and animation, for example, when doing an Custom Action or receiving a specific hitReaction ID")]
        [Tooltip("It will always automatically use the CancelReload")]
        public bool useCancelReload = true;
        [Tooltip("This is a list of HitReaction ID that will be ignored by the CancelReload routine")]
        public List<int> ignoreReacionIDList = new List<int>() { -1 };

        [vEditorToolbar("Aim")]
        [Header("- Float Values")]
        [Tooltip("min distance to aim")]
        public float minDistanceToAim = 1;
        public float checkAimRadius = 0.1f;
        [Tooltip("Check true to make the character always aim and walk on strafe mode")]
        [Header("- Shooter Settings")]
        public bool alwaysAiming;
        public bool onlyWalkWhenAiming = true;
        public bool useDefaultMovesetWhenNotAiming = true;

        [vEditorToolbar("IK Adjust")]        
        [Tooltip("smooth of the right hand when correcting the aim")]
        public float smoothArmIKRotation = 30f;
        [Tooltip("smooth of the right arm when correcting the aim")]
        public float smoothArmAlignWeight = 4f;
        [Tooltip("Limit the maxAngle for the right hand to correct the aim")]
        public float maxAimAngle = 60f;
        [Tooltip("Check this to syinc the weapon aim to the camera aim")]
        public bool raycastAimTarget = true;
        [Tooltip("Move camera angle when shot using recoil properties of weapon")]
        public bool applyRecoilToCamera = true;        
        [Tooltip("Check this to use IK on the left hand")]
        public bool useLeftIK = true, useRightIK = true;
        [Header("--- Start PlayMode to edit the IK Adjust ---")]
        public vWeaponIKAdjustList weaponIKAdjustList;

        [vEditorToolbar("Ammo UI")]
        [Tooltip("Use the vAmmoDisplay to shot ammo count")]
        public bool useAmmoDisplay = true;
        [Tooltip("ID to find ammoDisplay for leftWeapon")]
        public int leftWeaponAmmoDisplayID = -1;
        [Tooltip("ID to find ammoDisplay for rightWeapon")]
        public int rightWeaponAmmoDisplayID = 1;

        [vEditorToolbar("LockOn")]
        [Header("- LockOn (need the shooter lockon component)")]
        [Tooltip("Allow the use of the LockOn or not")]
        public bool useLockOn = false;
        [Tooltip("Allow the use of the LockOn only with a Melee Weapon")]
        public bool useLockOnMeleeOnly = true;

        [vEditorToolbar("HipFire")]
        [Header("- HipFire Options")]
        [Tooltip("If enable, remember to change your weak attack input to other input - this allows shot without aim")]
        public bool hipfireShot = false;
        [Tooltip("Precision of the weapon when shooting using hipfire (without aiming)")]
        public float hipfireDispersion = 0.5f;
        [Tooltip("Time to keep aiming after shot")]
        [SerializeField] protected float hipfireAimTime = 2f;
        [vEditorToolbar("Camera Sway")]
        [Header("- Camera Sway Settings")]
        [Tooltip("Camera Sway movement while aiming")]
        public float cameraMaxSwayAmount = 2f;
        [Tooltip("Camera Sway Speed while aiming")]
        public float cameraSwaySpeed = .5f;

        [vEditorToolbar("Weapons")]
        public vShooterWeapon rWeapon, lWeapon;
        public int reloadAnimatorLayer = 4;
        [HideInInspector]
        public vAmmoManager ammoManager;
        public TotalAmmoHandler totalAmmoHandler;
        public OnReloadWeapon onStartReloadWeapon;
        public OnReloadWeapon onFinishReloadWeapon;
        [HideInInspector]
        public vAmmoDisplay ammoDisplayR, ammoDisplayL;
        [HideInInspector]
        public vCamera.vThirdPersonCamera tpCamera;
        [HideInInspector]
        public bool showCheckAimGizmos;

        private Animator animator;
        private int totalAmmo;
        private int secundaryTotalAmmo;
        private bool usingThirdPersonController;
        private float currentShotTime;
        private float hipfirePrecisionAngle;
        private float hipfirePrecision;
        internal bool isReloadingWeapon;
        private bool cancelReload;
        protected vWeaponIKAdjust currentWeaponIKAdjust;

        internal readonly int IsShoot = Animator.StringToHash("Shoot");
        internal readonly int Reload = Animator.StringToHash("Reload");
        internal readonly int ReloadID = Animator.StringToHash("ReloadID");
        #endregion

        public virtual void Start()
        {
            animator = GetComponent<Animator>();
            if (applyRecoilToCamera)
                tpCamera = FindObjectOfType<vCamera.vThirdPersonCamera>();

            ammoManager = GetComponent<vAmmoManager>();
            if (ammoManager != null)
                ammoManager.updateTotalAmmo = new vAmmoManager.OnUpdateTotalAmmo(AmmoManagerWasUpdated);

            var tpInput = GetComponent<vThirdPersonController>();
            usingThirdPersonController = tpInput;

            if (usingThirdPersonController && useCancelReload)
                tpInput.onReceiveDamage.AddListener(CancelReload);

            if (useAmmoDisplay)
                GetAmmoDisplays();


            if (animator)
            {
                var _rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                var _lefttHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                var weaponR = _rightHand.GetComponentInChildren<vShooterWeapon>();
                var weaponL = _lefttHand.GetComponentInChildren<vShooterWeapon>();
                if (weaponR != null)
                    SetRightWeapon(weaponR.gameObject);
                if (weaponL != null)
                    SetLeftWeapon(weaponL.gameObject);
            }

            if (!ignoreTags.Contains(gameObject.tag))
                ignoreTags.Add(gameObject.tag);
            if (useAmmoDisplay)
            {
                if (ammoDisplayR) ammoDisplayR.UpdateDisplay("");
                if (ammoDisplayL) ammoDisplayL.UpdateDisplay("");
            }
            UpdateTotalAmmo();
        }

        public virtual void SetLeftWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponent<vShooterWeapon>();
                lWeapon = w;
                if (lWeapon)
                {
                    lWeapon.ignoreTags = ignoreTags;
                    lWeapon.hitLayer = damageLayer;
                    lWeapon.root = transform;
                    lWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (lWeapon.dontUseReload) LoadAllAmmo(lWeapon, false);
                    if (lWeapon.secundaryWeapon)
                    {
                        lWeapon.secundaryWeapon.ignoreTags = ignoreTags;
                        lWeapon.secundaryWeapon.hitLayer = damageLayer;
                        lWeapon.secundaryWeapon.root = transform;
                        lWeapon.secundaryWeapon.isSecundaryWeapon = true;
                        if (lWeapon.secundaryWeapon.dontUseReload) LoadAllAmmo(lWeapon.secundaryWeapon, true);
                    }
                    if (usingThirdPersonController)
                    {
                        if (useAmmoDisplay && !ammoDisplayL) GetAmmoDisplays();
                        if (useAmmoDisplay && ammoDisplayL) ammoDisplayL.Show();
                        UpdateLeftAmmo();
                    }
                    currentShotTime = 0;
                    LoadIKAdjust(lWeapon.weaponCategory);
                }
            }
        }

        public virtual void SetRightWeapon(GameObject weapon)
        {
            if (weapon != null)
            {
                var w = weapon.GetComponent<vShooterWeapon>();
                rWeapon = w;
                if (rWeapon)
                {
                    rWeapon.ignoreTags = ignoreTags;
                    rWeapon.hitLayer = damageLayer;
                    rWeapon.root = transform;
                    rWeapon.onDestroy.AddListener(OnDestroyWeapon);
                    if (rWeapon.dontUseReload) LoadAllAmmo(rWeapon, false);
                    if (rWeapon.secundaryWeapon)
                    {
                        rWeapon.secundaryWeapon.ignoreTags = ignoreTags;
                        rWeapon.secundaryWeapon.hitLayer = damageLayer;
                        rWeapon.secundaryWeapon.root = transform;
                        rWeapon.secundaryWeapon.isSecundaryWeapon = true;
                        if (rWeapon.secundaryWeapon.dontUseReload) LoadAllAmmo(rWeapon.secundaryWeapon, true);
                    }
                    if (usingThirdPersonController)
                    {
                        if (useAmmoDisplay && !ammoDisplayR) GetAmmoDisplays();
                        if (useAmmoDisplay && ammoDisplayR) ammoDisplayR.Show();
                        UpdateRightAmmo();
                    }
                    currentShotTime = 0;
                    LoadIKAdjust(rWeapon.weaponCategory);

                }
            }
        }

        public virtual void OnDestroyWeapon(GameObject otherGameObject)
        {
            if (usingThirdPersonController)
            {
                var ammoDisplay = rWeapon != null && otherGameObject == rWeapon.gameObject ? ammoDisplayR : lWeapon != null && otherGameObject == lWeapon.gameObject ? ammoDisplayL : null;

                if (useAmmoDisplay && ammoDisplay)
                {
                    ammoDisplay.UpdateDisplay("");
                    ammoDisplay.Hide();
                }
            }
            currentShotTime = 0;
        }

        protected virtual void GetAmmoDisplays()
        {
            var ammoDisplays = FindObjectsOfType<vAmmoDisplay>();
            if (ammoDisplays.Length > 0)
            {
                if (!ammoDisplayL)
                    ammoDisplayL = ammoDisplays.vToList().Find(d => d.displayID == leftWeaponAmmoDisplayID);
                if (!ammoDisplayR)
                    ammoDisplayR = ammoDisplays.vToList().Find(d => d.displayID == rightWeaponAmmoDisplayID);
            }
        }

        public virtual int GetMoveSetID()
        {
            int id = 0;

            if (rWeapon && rWeapon.gameObject.activeInHierarchy)
                id = (int)rWeapon.moveSetID;
            else if (lWeapon && lWeapon.gameObject.activeInHierarchy)
                id = (int)lWeapon.moveSetID;

            return id;
        }

        public virtual int GetUpperBodyID()
        {
            int id = 0;

            if (rWeapon && rWeapon.gameObject.activeInHierarchy)
                id = (int)rWeapon.upperBodyID;
            else if (lWeapon && lWeapon.gameObject.activeInHierarchy)
                id = (int)lWeapon.upperBodyID;

            return id;
        }

        public virtual int GetShotID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeInHierarchy) id = (int)rWeapon.shotID;
            else if (lWeapon && lWeapon.gameObject.activeInHierarchy) id = (int)lWeapon.shotID;
            return id;
        }

        public virtual int GetEquipID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeInHierarchy) id = (int)rWeapon.equipID;
            else if (lWeapon && lWeapon.gameObject.activeInHierarchy) id = (int)lWeapon.equipID;
            return id;
        }

        public virtual int GetReloadID()
        {
            int id = 0;
            if (rWeapon && rWeapon.gameObject.activeInHierarchy) id = (int)rWeapon.reloadID;
            else if (lWeapon && lWeapon.gameObject.activeInHierarchy) id = (int)lWeapon.reloadID;
            return id;
        }

        public float HipfireAimTime
        {
            get
            {
                return hipfireAimTime + (CurrentWeapon ? CurrentWeapon.shootFrequency : 0);

            }
        }

        public virtual bool WeaponHasLoadedAmmo(bool secundaryWeapon = false)
        {
            var hasAmmo = secundaryWeapon ? (CurrentWeapon && CurrentWeapon.secundaryWeapon ? CurrentWeapon.secundaryWeapon.ammoCount : 0) > 0 :
                (CurrentWeapon ? CurrentWeapon.ammoCount : 0) > 0;
            return hasAmmo;
        }

        public virtual bool WeaponHasUnloadedAmmo(bool secundaryWeapon = false)
        {
            var hasAmmo = secundaryWeapon ? secundaryTotalAmmo > 0 : totalAmmo > 0;
            return hasAmmo;
        }

        public virtual bool isShooting
        {
            get { return currentShotTime > 0; }
        }

        bool isReloading;
        public virtual void ReloadWeapon()
        {
            var weapon = rWeapon ? rWeapon : lWeapon;

            if (!weapon || !weapon.gameObject.activeInHierarchy || isReloading) return;
            UpdateTotalAmmo();
            bool primaryWeaponAnim = false;

            if (weapon.ammoCount < weapon.clipSize && (weapon.isInfinityAmmo || WeaponHasUnloadedAmmo()) && !weapon.dontUseReload)
            {
                onStartReloadWeapon.Invoke(weapon);

                if (animator)
                {
                    animator.SetInteger(ReloadID, GetReloadID());
                    animator.SetTrigger(Reload);
                }
                if (CurrentWeapon && CurrentWeapon.gameObject.activeInHierarchy) StartCoroutine(AddAmmoToWeapon(CurrentWeapon, CurrentWeapon.reloadTime));
                primaryWeaponAnim = true;
            }
            if (weapon.secundaryWeapon && weapon.secundaryWeapon.ammoCount >= weapon.secundaryWeapon.clipSize && (weapon.secundaryWeapon.isInfinityAmmo || WeaponHasUnloadedAmmo(true)) && !weapon.secundaryWeapon.dontUseReload)
            {
                if (!primaryWeaponAnim)
                {
                    if (animator)
                    {
                        primaryWeaponAnim = true;
                        animator.SetInteger(ReloadID, weapon.secundaryWeapon.reloadID);
                        animator.SetTrigger(Reload);
                    }
                }
                StartCoroutine(AddAmmoToWeapon(CurrentWeapon.secundaryWeapon, primaryWeaponAnim ? CurrentWeapon.reloadTime : CurrentWeapon.secundaryWeapon.reloadTime, !primaryWeaponAnim));
            }
        }

        protected virtual IEnumerator AddAmmoToWeapon(vShooterWeapon weapon, float delayTime, bool ignoreEffects = false)
        {
            isReloading = true;
            if (weapon.ammoCount < weapon.clipSize && (weapon.isInfinityAmmo || WeaponHasUnloadedAmmo()) && !weapon.dontUseReload && !cancelReload)
            {
                if (!ignoreEffects) weapon.ReloadEffect();
                yield return new WaitForSeconds(delayTime);
                if (!cancelReload)
                {
                    var needAmmo = weapon.reloadOneByOne ? 1 : weapon.clipSize - weapon.ammoCount;

                    if (weapon.isInfinityAmmo)
                    {
                        weapon.AddAmmo(needAmmo);
                    }
                    else
                    {
                        if (WeaponAmmo(weapon).count < needAmmo)
                            needAmmo = WeaponAmmo(weapon).count;
                        weapon.AddAmmo(needAmmo);
                        WeaponAmmo(weapon).Use(needAmmo);
                    }

                    if (weapon.reloadOneByOne && weapon.ammoCount < weapon.clipSize && WeaponHasUnloadedAmmo())
                    {
                        if (WeaponAmmo(weapon).count == 0)
                        {
                            if (!ignoreEffects) weapon.FinishReloadEffect();
                            if (!ignoreEffects) isReloadingWeapon = false;
                            if (!ignoreEffects) onFinishReloadWeapon.Invoke(weapon);
                        }
                        else
                        {
                            if (!ignoreEffects) isReloadingWeapon = true;
                            if (!cancelReload)
                            {
                                if (!ignoreEffects)
                                {
                                    animator.SetInteger(ReloadID, weapon.reloadID);
                                    animator.SetTrigger(Reload);
                                }
                                StartCoroutine(AddAmmoToWeapon(weapon, delayTime, ignoreEffects));
                            }
                        }
                    }
                    else
                    {
                        if (!ignoreEffects) weapon.FinishReloadEffect();
                        if (!ignoreEffects) isReloadingWeapon = false;
                        if (!ignoreEffects) onFinishReloadWeapon.Invoke(weapon);
                    }
                }
                UpdateTotalAmmo();
            }
            isReloading = false;
        }

        public virtual void CancelReload()
        {
            StartCoroutine(CancelReloadRoutine());
        }

        public virtual void CancelReload(vDamage damage)
        {
            if (!ignoreReacionIDList.Contains(damage.reaction_id) && isReloading)
                StartCoroutine(CancelReloadRoutine());
        }

        protected virtual IEnumerator CancelReloadRoutine()
        {
            if (CurrentWeapon != null)
            {
                animator.ResetTrigger("Reload");
                cancelReload = true;
                StopCoroutine("AddAmmoToWeapon");
                yield return new WaitForSeconds(CurrentWeapon.reloadTime + 0.1f);
                cancelReload = false;
                if (isReloadingWeapon)
                {
                    isReloadingWeapon = false;
                    if (CurrentWeapon)
                        onFinishReloadWeapon.Invoke(CurrentWeapon);
                }
                UpdateTotalAmmo();
            }
        }

        public virtual void LoadAllAmmo(vShooterWeapon weapon, bool secundaryWeapon = false)
        {
            if (!weapon) return;
            UpdateTotalAmmo();
            if (weapon.ammoCount < weapon.clipSize && (weapon.isInfinityAmmo || WeaponHasUnloadedAmmo(secundaryWeapon)))
            {
                var needAmmo = weapon.clipSize - weapon.ammoCount;
                if (weapon.isInfinityAmmo)
                    weapon.AddAmmo(needAmmo);
                else
                {
                    if (WeaponAmmo(weapon).count < needAmmo)
                        needAmmo = WeaponAmmo(weapon).count;
                    weapon.AddAmmo(needAmmo);
                    WeaponAmmo(weapon).Use(needAmmo);
                }
                weapon.onReload.Invoke();
            }
        }

        public virtual vAmmo WeaponAmmo(vShooterWeapon weapon)
        {
            if (!weapon) return null;
            var ammo = new vAmmo();
            if (ammoManager && ammoManager.ammos != null && ammoManager.ammos.Count > 0)
            {
                ammo = ammoManager.GetAmmo(weapon.ammoID);
            }
            return ammo;
        }

        public virtual vShooterWeapon CurrentWeapon
        {
            get
            {
                var _weapon = rWeapon ?
                    rWeapon :
                    lWeapon ?
                    lWeapon : null;
                return _weapon;
            }
        }

        public virtual vWeaponIKAdjust CurrentWeaponIK
        {
            get
            {
                return currentWeaponIKAdjust;
            }
        }
        public virtual void LoadIKAdjust(string category)
        {
            if (weaponIKAdjustList)
                currentWeaponIKAdjust = weaponIKAdjustList.GetWeaponIK(category);
        }

        public virtual void SetIKAdjust(vWeaponIKAdjust iKAdjust)
        {
            currentWeaponIKAdjust = iKAdjust;
        }
        public virtual bool IsLeftWeapon
        {
            get
            {
                var isLeftWp = (rWeapon == null) ?
                    (lWeapon) : false;
                return isLeftWp;
            }
        }

        public virtual void AmmoManagerWasUpdated()
        {
            bool needUpdateAmmo = true;
            if (CurrentWeapon)
            {
                if (CurrentWeapon.dontUseReload)
                {
                    LoadAllAmmo(CurrentWeapon, false);
                    needUpdateAmmo = false;
                }
                if (CurrentWeapon.secundaryWeapon && CurrentWeapon.secundaryWeapon.dontUseReload)
                {
                    LoadAllAmmo(CurrentWeapon.secundaryWeapon, true);
                    needUpdateAmmo = false;
                }

            }
            if (needUpdateAmmo) UpdateTotalAmmo();
        }

        public virtual void UpdateTotalAmmo()
        {
            UpdateLeftAmmo();
            UpdateRightAmmo();
        }

        public virtual void UpdateLeftAmmo()
        {
            if (!lWeapon) return;
            UpdateTotalAmmo(lWeapon, ref totalAmmo, -1);
            UpdateTotalAmmo(lWeapon.secundaryWeapon, ref secundaryTotalAmmo, -1);
        }

        public virtual bool IsCurrentWeaponActive()
        {
            return CurrentWeapon && CurrentWeapon.gameObject.activeInHierarchy;
        }

        public virtual void UpdateRightAmmo()
        {
            if (!rWeapon) return;
            UpdateTotalAmmo(rWeapon, ref totalAmmo, 1);
            UpdateTotalAmmo(rWeapon.secundaryWeapon, ref secundaryTotalAmmo, 1);
        }

        protected virtual void UpdateTotalAmmo(vShooterWeapon weapon, ref int targetTotalAmmo, int displayId)
        {
            if (!weapon) return;

            var ammoCount = 0;
            if (weapon.isInfinityAmmo) ammoCount = 9999;
            else
            {
                var ammo = WeaponAmmo(weapon);
                if (ammo != null) ammoCount += ammo.count;
            }
            targetTotalAmmo = ammoCount;
            UpdateAmmoDisplay(displayId);
        }

        protected virtual void UpdateAmmoDisplay(int displayId)
        {
            if (!useAmmoDisplay) return;
            var weapon = displayId == 1 ? rWeapon : lWeapon;

            if (!weapon) return;
            if (!ammoDisplayR || !ammoDisplayL) GetAmmoDisplays();
            var ammoDisplay = displayId == 1 ? ammoDisplayR : ammoDisplayL;
            if (useAmmoDisplay && ammoDisplay)
            {
                if (weapon.secundaryWeapon)
                {
                    string display1 = weapon.dontUseReload ? weapon.isInfinityAmmo ? "Infinity" : (weapon.ammoCount + totalAmmo).ToString() : (string.Format("{0} / {1}", weapon.ammoCount, (weapon.isInfinityAmmo ? "Infinity" : totalAmmo.ToString())));
                    string displat2 = weapon.secundaryWeapon.dontUseReload ? weapon.secundaryWeapon.isInfinityAmmo ? "Infinity" : (weapon.secundaryWeapon.ammoCount + secundaryTotalAmmo).ToString() : (string.Format("{0} / {1}", weapon.secundaryWeapon.ammoCount, (weapon.secundaryWeapon.isInfinityAmmo ? "Infinity" : secundaryTotalAmmo.ToString())));
                    ammoDisplay.UpdateDisplay(display1 + "\n" + displat2, weapon.ammoID);
                }
                else
                {
                    string text = weapon.dontUseReload ? weapon.isInfinityAmmo ? "Infinity" : (weapon.ammoCount + totalAmmo).ToString() : (string.Format("{0} / {1}", weapon.ammoCount, (weapon.isInfinityAmmo ? "Infinity" : totalAmmo.ToString())));
                    ammoDisplay.UpdateDisplay(text, weapon.ammoID);
                }

            }
        }

        public virtual void Shoot(Vector3 aimPosition, bool applyHipfirePrecision = false, bool useSecundaryWeapon = false)
        {
            if (isShooting) return;

            var weapon = rWeapon ? rWeapon : lWeapon;
            if (!weapon || !weapon.gameObject.activeInHierarchy) return;
            var secundaryWeapon = weapon.secundaryWeapon;

            if (useSecundaryWeapon && !secundaryWeapon)
            {
                return;
            }
            var targetWeapon = useSecundaryWeapon ? secundaryWeapon : weapon;
            if (targetWeapon.dontUseReload)
            {
                LoadAllAmmo(targetWeapon, useSecundaryWeapon);
            }
            else if (targetWeapon.autoReload && targetWeapon.ammoCount <= 0 && WeaponHasUnloadedAmmo())
            {
                ReloadWeapon();
                return;
            }

            var _aimPos = applyHipfirePrecision ? aimPosition + HipFirePrecision(aimPosition) : aimPosition;
            var applyRecoil = false;
            targetWeapon.Shoot(_aimPos, transform, (bool sucessful) => { applyRecoil = sucessful; });

            if (applyRecoil)
            {
                var recoilHorizontal = Random.Range(targetWeapon.recoilLeft, targetWeapon.recoilRight);
                var recoilUp = Random.Range(0, targetWeapon.recoilUp);
                StartCoroutine(Recoil(recoilHorizontal, recoilUp));
            }

            UpdateAmmoDisplay(rWeapon ? 1 : -1);

            if (targetWeapon.dontUseReload)
            {
                LoadAllAmmo(targetWeapon, useSecundaryWeapon);
            }
            currentShotTime = weapon.shootFrequency;
            if (!useSecundaryWeapon && totalAmmo <= 0)
            {
                weapon.onFinishAmmo.Invoke();
            }
            else if (useSecundaryWeapon && secundaryTotalAmmo <= 0 && weapon.secundaryWeapon) weapon.secundaryWeapon.onFinishAmmo.Invoke();
        }

        protected virtual IEnumerator Recoil(float horizontal, float up)
        {
            yield return new WaitForSeconds(0.02f);
            if (animator)
            {
                animator.SetTrigger(IsShoot);
            }
            if (tpCamera != null && applyRecoilToCamera) tpCamera.RotateCamera(horizontal, up);
        }

        protected virtual Vector3 HipFirePrecision(Vector3 _aimPosition)
        {
            var weapon = rWeapon ? rWeapon : lWeapon;
            if (!weapon) return Vector3.zero;
            hipfirePrecisionAngle = UnityEngine.Random.Range(-1000, 1000);
            hipfirePrecision = Random.Range(-hipfireDispersion, hipfireDispersion);
            var dir = (Quaternion.AngleAxis(hipfirePrecisionAngle, _aimPosition - weapon.muzzle.position) * (Vector3.up)).normalized * hipfirePrecision;
            return dir;
        }

        public virtual void CameraSway()
        {
            var weapon = rWeapon ? rWeapon : lWeapon;
            if (!weapon) return;
            float bx = (Mathf.PerlinNoise(0, Time.time * cameraSwaySpeed) - 0.5f);
            float by = (Mathf.PerlinNoise(0, (Time.time * cameraSwaySpeed) + 100)) - 0.5f;

            var swayAmount = cameraMaxSwayAmount * (1f - weapon.precision);
            if (swayAmount == 0) return;

            bx *= swayAmount;
            by *= swayAmount;

            float tx = (Mathf.PerlinNoise(0, Time.time * cameraSwaySpeed) - 0.5f);
            float ty = ((Mathf.PerlinNoise(0, (Time.time * cameraSwaySpeed) + 100)) - 0.5f);

            tx *= -(swayAmount * 0.25f);
            ty *= (swayAmount * 0.25f);

            if (tpCamera != null)
            {
                vCamera.vThirdPersonCamera.instance.offsetMouse.x = bx + tx;
                vCamera.vThirdPersonCamera.instance.offsetMouse.y = by + ty;
            }
        }

        public virtual void UpdateShotTime()
        {
            if (currentShotTime > 0) currentShotTime -= Time.deltaTime;
        }
    }
}