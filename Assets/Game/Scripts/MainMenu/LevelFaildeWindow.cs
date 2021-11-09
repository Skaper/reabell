using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelFaildeWindow : MonoBehaviour
{
    public GameObject windowCanas;
    public TextMeshProUGUI textInfo;


    // Start is called before the first frame update
    void Start()
    {

        if (CrossSceneInformation.DeadInfo != null)
        {
            windowCanas.SetActive(true);
            textInfo.text = CrossSceneInformation.DeadInfo;
            CrossSceneInformation.DeadInfo = null;
        }
        else
        {
            windowCanas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CloseWindow()
    {
        windowCanas.SetActive(false);
    }
}
