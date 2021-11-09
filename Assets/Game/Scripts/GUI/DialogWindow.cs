using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
public class DialogWindow : MonoBehaviour
{
    public GameObject windowCanas;
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;

    public MenuButton yesButton;
    public MenuButton noButton;

    [SerializeField]
    private bool isActiveOnStart;

    public string TitleText;
    public string MessageText;

    public UnityEvent onYesClicked;
    public UnityEvent onNoClicked;

    void Start()
    {

        if(isActiveOnStart)
        {
            Show(TitleText, MessageText);
        }
        else
        {
            Hide();
        }
        yesButton.onClick.AddListener(yesClicked);
        noButton.onClick.AddListener(noClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(string _title, string _message)
    {
        windowCanas.SetActive(true);
        TitleText = _title;
        MessageText = _message;
        title.text = TitleText;
        text.text = MessageText;
    }

    public void Hide()
    {
        windowCanas.SetActive(false);
    }

    private void yesClicked()
    {
        onYesClicked?.Invoke();
    }

    private void noClicked()
    {
        onNoClicked?.Invoke();
        Hide();
    }
}
