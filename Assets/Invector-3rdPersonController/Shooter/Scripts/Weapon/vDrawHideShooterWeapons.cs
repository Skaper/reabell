﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vShooter
{
    using vCharacterController;

    [vClassHeader("Draw/Hide Shooter Melee Weapons", "This component works with vItemManager, vWeaponHolderManager and vShooterMeleeInput", useHelpBox = true)]
    public class vDrawHideShooterWeapons : vDrawHideMeleeWeapons
    {
        vShooterMeleeInput shooter;

        protected override void Start()
        {
            base.Start();
            shooter = GetComponent<vShooterMeleeInput>();
        }

        protected override bool CanHideWeapons()
        {
            return (shooter && shooter.shooterManager && shooter.shooterManager.CurrentWeapon && (forceHide || (!shooter._isAiming && shooter._aimTimming<=0 && !shooter.isReloading)))
                    || (base.CanHideWeapons() && (forceHide || (!shooter._isAiming && shooter._aimTimming <= 0 && !shooter.isReloading)));
        }

        protected override bool CanDrawWeapons()
        {
            return (shooter && shooter.shooterManager && shooter.shooterManager.CurrentWeapon && !shooter.shooterManager.CurrentWeapon.gameObject.activeInHierarchy) || base.CanDrawWeapons();
        }

        protected override GameObject RightWeaponObject(bool checkIsActve = false)
        {
            if (shooter && shooter.shooterManager && shooter.shooterManager.rWeapon && (!checkIsActve || shooter.shooterManager.rWeapon.gameObject.activeInHierarchy))
                return shooter.shooterManager.rWeapon.gameObject;
            return base.RightWeaponObject(checkIsActve);
        }

        protected override GameObject LeftWeaponObject(bool checkIsActve = false)
        {
            if (shooter && shooter.shooterManager && shooter.shooterManager.lWeapon && (!checkIsActve || shooter.shooterManager.lWeapon.gameObject.activeInHierarchy))
                return shooter.shooterManager.lWeapon.gameObject;
            return base.LeftWeaponObject(checkIsActve);
        }

        protected override void DrawRightWeapon(bool immediate = false)
        {
            base.DrawRightWeapon(immediate);
        }

        protected override bool DrawWeaponsImmediateConditions()
        {
            if (shooter && shooter.shooterManager && shooter.shooterManager.CurrentWeapon) return DrawShooterWeaponImmediateConditions();            
            return base.DrawWeaponsImmediateConditions();
        }

        protected virtual bool DrawShooterWeaponImmediateConditions()
        {
            if (!shooter || !shooter.shooterManager || shooter.cc.customAction || !shooter.shooterManager.CurrentWeapon) return false;
            if (shooter.CurrentActiveWeapon == null && (shooter.aimInput.GetButtonDown() || (shooter.shooterManager.hipfireShot && shooter.shotInput.GetButtonDown()))) return true;

            return false;
        }

        protected override void HandleInput()
        {
            base.HandleInput();
            HandleShooterInput();
        }

        protected virtual void HandleShooterInput()
        {
            if (!shooter.cc.IsAnimatorTag("IsThrowing") && shooter && shooter.shooterManager && !shooter.cc.customAction && shooter.shooterManager.CurrentWeapon && shooter.CurrentActiveWeapon == null && !shooter._isAiming && !shooter.shooterManager.hipfireShot && shooter.shotInput.GetButtonDown())
            {
                if (!IsEquipping)
                {
                    if (CanHideRightWeapon() || CanHideLeftWeapon())
                        HideWeapons();
                    else if (CanDrawRightWeapon() || CanDrawLeftWeapon())
                        DrawWeapons();
                }
            }
        }
    }
}