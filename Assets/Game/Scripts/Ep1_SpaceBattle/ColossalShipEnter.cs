using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColossalShipEnter : MonoBehaviour
{
    public PlayerShipController playerShip;
    public LevelChanger levelChanger;
    float timer;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timer += Time.fixedDeltaTime;
            if(timer > 1.5f)
            {
                playerShip.enabled = false;
                levelChanger.changeLevel = true;
                if (PlayerPrefs.GetString("CurrentProgress").Equals("Ep1SpaceBattle"))
                {
                    PlayerPrefs.SetString("CurrentProgress", "Ep2.1Ship");
                    PlayerPrefs.Save();
                }
                levelChanger.FadeToLevel("Ep2.1Ship");
                
            }
        }
    }
}
