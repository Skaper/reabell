using UnityEngine;

namespace Invector.vMelee
{
    using vCharacterController.vActions;
    using vShooter;

    [vClassHeader("Collect Shooter Melee Control", "This component is used when you're character doesn't have a ItemManager to manage items, this will allow you to pickup 1 weapon at the time.")]
    public class vCollectShooterMeleeControl : vCollectMeleeControl
    {
        protected vShooterManager shooterManager;

        protected override void Start()
        {
            base.Start();
            shooterManager = GetComponent<vShooterManager>();
        }

        public override void HandleCollectableInput(vCollectableStandalone collectableStandAlone)
        {
            if (shooterManager && collectableStandAlone != null && collectableStandAlone.weapon != null)
            {
                var weapon = collectableStandAlone.weapon.GetComponent<vShooterWeapon>();
                if (weapon)
                {
                    Transform p = null;
                    if (weapon.isLeftWeapon)
                    {
                        p = GetEquipPoint(leftHandler, collectableStandAlone.targetEquipPoint);
                        if (p)
                        {
                            collectableStandAlone.weapon.transform.SetParent(p);
                            collectableStandAlone.weapon.transform.localPosition = Vector3.zero;
                            collectableStandAlone.weapon.transform.localEulerAngles = Vector3.zero;

                            if (leftWeapon && leftWeapon != weapon.gameObject)
                                RemoveLeftWeapon();

                            shooterManager.SetLeftWeapon(weapon.gameObject);
                            collectableStandAlone.OnEquip.Invoke();
                            leftWeapon = weapon.gameObject;
                            UpdateLeftDisplay(collectableStandAlone);

                            if (rightWeapon)
                                RemoveRightWeapon();
                        }
                    }
                    else
                    {
                        p = GetEquipPoint(rightHandler, collectableStandAlone.targetEquipPoint);
                        if (p)
                        {
                            collectableStandAlone.weapon.transform.SetParent(p);
                            collectableStandAlone.weapon.transform.localPosition = Vector3.zero;
                            collectableStandAlone.weapon.transform.localEulerAngles = Vector3.zero;

                            if (rightWeapon && rightWeapon != weapon.gameObject)
                                RemoveRightWeapon();

                            shooterManager.SetRightWeapon(weapon.gameObject);
                            collectableStandAlone.OnEquip.Invoke();
                            rightWeapon = weapon.gameObject;
                            UpdateRightDisplay(collectableStandAlone);

                            if (leftWeapon)
                                RemoveLeftWeapon();
                        }
                    }
                }
            }
            base.HandleCollectableInput(collectableStandAlone);
        }

        public override void RemoveRightWeapon()
        {
            base.RemoveRightWeapon();
            if (shooterManager)
                shooterManager.rWeapon = null;
        }

        public override void RemoveLeftWeapon()
        {
            base.RemoveLeftWeapon();
            if (shooterManager)
                shooterManager.lWeapon = null;
        }
    }
}