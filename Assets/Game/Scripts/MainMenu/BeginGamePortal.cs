using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginGamePortal : MonoBehaviour
{
    private bool isOpen;
    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen && other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("CurrentProgress", "Ep1SpaceBattle");
            PlayerPrefs.Save();

            GameManager.PlayerReferences.LevelChanger.FadeToLevel("Ep1SpaceBattle");
            isOpen = true;
        }
    }
}
