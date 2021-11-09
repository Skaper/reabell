using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vShooter;
using BNG;
public class Shooting : GrabbableEvents
{
    public Animator weponAnimator;

    InputBridge input;
    vShooterWeapon weapon;

    private Grabber currentGrabber;
    // Start is called before the first frame update
    void Start()
    {
        input = InputBridge.Instance;
        weapon = gameObject.GetComponent<vShooterWeapon>();
        weapon.isInfinityAmmo = true;
        weapon.dontUseReload = true;
        weapon.ammo = int.MaxValue;
    }
    public override void OnRelease()
    {
        currentGrabber = null;
    }
    //Grab in hand, show VR Pointer
    public override void OnGrab(Grabber grabber)
    {
        currentGrabber = grabber;
        base.OnGrab(grabber);
    }

    public override void OnTriggerDown()
    {
        weapon.Shoot();
        input.VibrateController(1f, 1f, 0.3f, currentGrabber.HandSide);
        weapon.autoReload = true;
        if (weponAnimator != null) weponAnimator.SetTrigger("Shoot");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
