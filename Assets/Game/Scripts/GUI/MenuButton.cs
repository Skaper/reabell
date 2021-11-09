using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using BNG;
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool isEnabled = true;
    public WindowField connectedWindow;
    [Tooltip("highlight Then Connected Window is Open")]
    public bool highlightThenOpen = true;

    [SerializeField]
    private Animator animator;

    public string normalTrigger = "Normal";
    public string highlightedTrigger = "Highlighted";
    public string pressedTrigger = "Pressed";
    public string highlightedPressedTrigger = "HighlightedPressed";
    public string selectedTrigger = "Highlighted";
    public string disabledTrigger = "Disabled";

    public UnityEvent onClick;

    private bool t_isEnabled;
    private bool inFocus;
    private bool wasMenuOpen;

    private InputBridge input;
    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (connectedWindow == null) isEnabled = false;

        if (!isEnabled) animator.SetTrigger(disabledTrigger);
        t_isEnabled = isEnabled;

        input = InputBridge.Instance;
    }
    private void OnEnable()
    {
        if (!isEnabled) animator.SetTrigger(disabledTrigger);
    }
    void Update()
    {
        if (t_isEnabled != isEnabled)
        {
            t_isEnabled = isEnabled;
            if (!isEnabled) animator.SetTrigger(disabledTrigger);
        }
        if (connectedWindow != null)
        {
            bool isMenuOpen = connectedWindow.isWindowActive;
            if (wasMenuOpen && !isMenuOpen && !inFocus)
            {
                animator.SetTrigger(normalTrigger);
            }
            wasMenuOpen = isMenuOpen;
        }

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEnabled)
        {
            onClick?.Invoke();
            if (wasMenuOpen)
            {
                connectedWindow.HideWindow();
            }
            else
            {
                connectedWindow.ShowWindow();
            }
            animator.SetTrigger(pressedTrigger);

        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnabled)
        {
            inFocus = true;
            if (highlightThenOpen)
            {
                if (!wasMenuOpen) animator.SetTrigger(highlightedTrigger);
                else animator.SetTrigger(highlightedPressedTrigger);
            }
            else
            {
                animator.SetTrigger(highlightedTrigger);
            }

        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnabled)
        {
            inFocus = false;
            if (highlightThenOpen)
            {
                if (!wasMenuOpen) animator.SetTrigger(normalTrigger);
            }
            else
            {
                animator.SetTrigger(normalTrigger);
            }

        }

    }

 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft"))
        {
            input.VibrateController(0.1f, 0.5f, 0.2f, ControllerHand.Left);
            onButtonClick();
        }else if (other.CompareTag("Player/FingerRight"))
        {
            input.VibrateController(0.1f, 0.5f, 0.2f, ControllerHand.Right);
            onButtonClick();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft"))
        {
            input.VibrateController(0.1f, 0.1f, 0.2f, ControllerHand.Left);
            onExit();
        }
        else if (other.CompareTag("Player/FingerRight"))
        {
            input.VibrateController(0.1f, 0.1f, 0.2f, ControllerHand.Right);
            onExit();
        }
    }
    private void onExit()
    {
        if (isEnabled)
        {
            inFocus = false;
            if (highlightThenOpen)
            {
                //if (!wasMenuOpen) animator.SetTrigger(normalTrigger);
            }
            else
            {
                //animator.SetTrigger(normalTrigger);
            }

        }
    }
    private void onButtonClick()
    {
        if (isEnabled)
        {
            onClick?.Invoke();
            if (wasMenuOpen)
            {
                connectedWindow.HideWindow();
                animator.SetTrigger(normalTrigger);
            }
            else
            {
                connectedWindow.ShowWindow();
                animator.SetTrigger(pressedTrigger);
            }
            //animator.SetTrigger(pressedTrigger);

        }
    }
}