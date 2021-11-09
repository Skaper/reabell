using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;
using BNG;

public class VRButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public bool isEnabled = true;

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

    private InputBridge input;
    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (!isEnabled) animator.SetTrigger(disabledTrigger);
        t_isEnabled = isEnabled;
        input = InputBridge.Instance;
    }

    private void Update()
    {
        if (t_isEnabled != isEnabled)
        {
            t_isEnabled = isEnabled;
            if (!isEnabled) animator.SetTrigger(disabledTrigger);
        }
    }

    private void OnEnable()
    {
        if (!isEnabled) animator.SetTrigger(disabledTrigger);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEnabled)
        {
            onClick?.Invoke();
            animator.SetTrigger(pressedTrigger);

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEnabled)
        {
            inFocus = true;
            animator.SetTrigger(highlightedTrigger);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEnabled)
        {
            inFocus = false;
            animator.SetTrigger(normalTrigger);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player/FingerLeft"))
        {
            input.VibrateController(0.1f, 0.5f, 0.2f, ControllerHand.Left);
            onButtonClick();
        }
        else if (other.CompareTag("Player/FingerRight"))
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

    private void onButtonClick()
    {
        if (isEnabled)
        {
            animator.SetTrigger(pressedTrigger);
            onClick?.Invoke();
        }
    }

    private void onExit()
    {
        if (isEnabled)
        {
            inFocus = false;
            animator.SetTrigger(normalTrigger);

        }
    }
}
