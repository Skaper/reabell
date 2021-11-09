using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWindowController : MonoBehaviour
{
    public VRButton ep1;
    public VRButton ep1_1;
    public VRButton ep2;
    // Start is called before the first frame update
    void Start()
    {
        string currentLevel = PlayerPrefs.GetString("CurrentProgress");

        switch (currentLevel)
        {
            case "Ep1SpaceBattle":
                ep1.isEnabled = true;
                ep1_1.isEnabled = false;
                ep2.isEnabled = false;
                break;
            case "Ep2.1Ship":
                ep1.isEnabled = true;
                ep1_1.isEnabled = true;
                ep2.isEnabled = false;
                break;
            case "Ep2CargoStation":
                ep1.isEnabled = true;
                ep1_1.isEnabled = true;
                ep2.isEnabled = true;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
