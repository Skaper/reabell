using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;
public class FlyTutorialPlace : MonoBehaviour
{
    public GameObject leftController;
    public GameObject rightController;

    public Transform leftHoldPoint;
    public Transform rightHoldPoint;

    public AdvancedWalkerController walkerController;
    public PlayerRotationProvider playerRotationProvider;

    public Animator flyTutorialAnimator;
    public Animator mainDoorAnimator;

    private Pose leftControllerDefaultPose;
    private Pose rightControllerDefaultPose;

    private bool isDoorOpen;
    

    private bool isControllersOnHand;
    void Start()
    {
        flyTutorialAnimator.Play("FlyTutorialHide");
        leftControllerDefaultPose = new Pose
        {
            position = leftController.transform.position,
            rotation = leftController.transform.rotation
        };
        rightControllerDefaultPose = new Pose
        {
            position = rightController.transform.position,
            rotation = rightController.transform.rotation
        };

        if (PlayerPrefs.HasKey("CurrentProgress") &&
            ! PlayerPrefs.GetString("CurrentProgress").Equals("MainMenu"))
        {
            mainDoorAnimator.Play("Doorframe_02_open");
            isDoorOpen = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float stayDelay = 0.1f;
    private float elapsed;
    private bool canGrub = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft") || other.CompareTag("Player/FingerRight"))
        {
            elapsed += Time.fixedDeltaTime;
            if (canGrub && elapsed > stayDelay)
            {
                StandWork();
                canGrub = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft") || other.CompareTag("Player/FingerRight"))
        {
            elapsed = 0;
            canGrub = true;

        }
    }

    private void StandWork()
    {
        if (!isControllersOnHand)
        {
            leftController.transform.SetParent(leftHoldPoint);
            rightController.transform.SetParent(rightHoldPoint);

            leftController.transform.localPosition = Vector3.zero;
            leftController.transform.rotation = leftHoldPoint.rotation;
            rightController.transform.localPosition = Vector3.zero;
            rightController.transform.rotation = rightHoldPoint.rotation;

            walkerController.enabled = false;
            playerRotationProvider.enabled = false;

            flyTutorialAnimator.Play("FlyTutorial");

            isControllersOnHand = true;
        }
        else
        {
            leftController.transform.SetParent(transform);
            rightController.transform.SetParent(transform);

            leftController.transform.position = leftControllerDefaultPose.position;
            leftController.transform.rotation = leftControllerDefaultPose.rotation;
            rightController.transform.position = rightControllerDefaultPose.position;
            rightController.transform.rotation = rightControllerDefaultPose.rotation;

            walkerController.enabled = true;
            playerRotationProvider.enabled = true;

            flyTutorialAnimator.Play("FlyTutorialClose");
            if(!isDoorOpen) mainDoorAnimator.Play("Doorframe_02_open");
            isControllersOnHand = false;
        }
    }
}
