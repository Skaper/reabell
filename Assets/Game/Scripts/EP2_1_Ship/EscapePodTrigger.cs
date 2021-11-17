using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodTrigger : MonoBehaviour
{
    private bool isOpen;
    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen && other.CompareTag("Player"))
        {
            if (PlayerPrefs.GetString("CurrentProgress").Equals("Ep2.1Ship"))
            {
                PlayerPrefs.SetString("CurrentProgress", "Ep2CargoStation");
                PlayerPrefs.Save();
            }
            
            GameManager.PlayerReferences.LevelChanger.FadeToLevel("Ep2CargoStation");
            isOpen = true;
        }
    }
}
