using BNG;
using UnityEngine;

public class PlayerShipWeapon : MonoBehaviour
{
    [Header("Weapon settings")]
    public bool isWeaponActive = true;
    public Transform weaponRay;
    public int rayLayout;
    public GameObject reticle;
    public Transform weaponPointer;
    public GameObject bullet;
    public float bulletForce = 50f;
    public PlayerShipController mainShipController;

    public Transform weaponPointLeft;
    public Transform weaponPointRight;

    [Header("Sound settings")]
    public AudioClip[] shootClips;
    public bool playeSingleClipOnce = true;
    public AudioSource shootAudio;

    private bool t_isWeaponActive = true;

    private bool isLeftWeaponShoot;
    private bool isRightWeaponShoot;

    private float shootRayCheckDistance = 4f;

    private InputBridge input;

    void Start()
    {
        reticle.SetActive(isWeaponActive);
        t_isWeaponActive = isWeaponActive;

        input = InputBridge.Instance;
    }

    void Update()
    {
        if(isWeaponActive != t_isWeaponActive)
        {
            reticle.SetActive(isWeaponActive);
            t_isWeaponActive = isWeaponActive;
        }
        if (!isWeaponActive) return;
        
        Vector3 camAngle = new Vector3(weaponPointer.eulerAngles.x, weaponPointer.eulerAngles.y, weaponPointer.eulerAngles.z);
        Quaternion deltaRotation = Quaternion.Euler(camAngle);
        gameObject.transform.rotation = deltaRotation;

        if (input.LeftTriggerDown)
        {
            if (CanWeaponShoot())
            {
                SimpleBullet simpleBullet = Instantiate(bullet, weaponPointLeft.position, Quaternion.identity).GetComponent<SimpleBullet>();
                simpleBullet.AddForce(transform.forward * (mainShipController.velocityMagnitude  + bulletForce));//(Mathf.Pow(mainShipController.velocityMagnitude, 1f / 3f) + bulletForce));
                simpleBullet.creatorTag = tag;
                isLeftWeaponShoot = true;
            }
            if (input.LeftGrip > 0.7f)
            {
                if (CanWeaponShoot())
                {
                    SimpleBullet simpleBullet = Instantiate(bullet, weaponPointRight.position, Quaternion.identity).GetComponent<SimpleBullet>();
                    simpleBullet.AddForce(transform.forward * (mainShipController.velocityMagnitude + bulletForce));
                    simpleBullet.creatorTag = tag;
                    isRightWeaponShoot = true;
                }
            }
            
        }
        
        if(isLeftWeaponShoot || isRightWeaponShoot)
        {
            isLeftWeaponShoot = false;
            isRightWeaponShoot = false;
            MakeShootSound();
        }
        Debug.DrawRay(
                weaponRay.position,
                weaponRay.TransformDirection(Vector3.forward) * shootRayCheckDistance,
                Color.red);
    }

    private bool CanWeaponShoot()
    {
        RaycastHit weaponHit;
        Physics.Raycast(
            weaponRay.position,
            weaponRay.TransformDirection(Vector3.forward),
            out weaponHit,
            shootRayCheckDistance,
            1 << rayLayout,
            QueryTriggerInteraction.Ignore
            );
        
        if (weaponHit.collider) return false;

        return true;
    }

    private void MakeShootSound()
    {
        if (playeSingleClipOnce)
        {
            shootAudio.clip = shootClips[Random.Range(0, shootClips.Length)];
            shootAudio.Play();
        }
        else if(!shootAudio.isPlaying)
        {
            shootAudio.clip = shootClips[Random.Range(0, shootClips.Length)];
            shootAudio.Play();
        }
    }
}
