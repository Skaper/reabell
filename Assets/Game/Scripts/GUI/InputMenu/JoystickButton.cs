using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private InputManager.KeyType keyType;
    [SerializeField]
    private TextMeshProUGUI textOnButton;
    [SerializeField]
    private Animator animator;
    public string normalTrigger = "Normal";
    public string highlightedTrigger = "Highlighted";

    public bool sendEventOnClick = false;

    public delegate void TextDelegate(string text);
    private TextDelegate textSetter;
    private bool isPointerStay = false;
    void Start()
    {
        textSetter = SetText;
        InputManager.OnActionLoadKey += OnKeyLoad;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPointerStay && !sendEventOnClick)
        {
            InputManager.OnActionSelectingKey?.Invoke(keyType, textSetter);
        }
    }

    private void SetText(string text)
    {
        textOnButton.SetText(text);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sendEventOnClick)
        {
            InputManager.OnActionSelectingKey?.Invoke(keyType, null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetTrigger(highlightedTrigger);
        isPointerStay = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetTrigger(normalTrigger);
        isPointerStay = false;
    }

    private void OnKeyLoad(InputManager.KeyType type, string name)
    {
        if (type == keyType) SetText(name);
    }
}
