using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPlayerPrefs : MonoBehaviour
{
    public bool createPrefs = false;
    public bool deletePrefs = false;

    public bool openAllLevels;
    void Awake()
    {
        CreatePrefs();
    }

    // Update is called once per frame
    void Update()
    {
        if (createPrefs)
        {
            CreatePrefs();
            createPrefs = false;
        }

        if (deletePrefs)
        {
            DeletePrefs();
            deletePrefs = false;
        }
        if (openAllLevels)
        {
            OpenAllLevels();
            openAllLevels = false;
        }
    }

    public void CreatePrefs()
    {
        if (!PlayerPrefs.HasKey("CurrentProgress"))
        {
            PlayerPrefs.SetString("CurrentProgress", "MainMenu");
        }

        if (!PlayerPrefs.HasKey("GlobalVolume"))
        {
            PlayerPrefs.SetFloat("GlobalVolume", 1f);
            PlayerPrefs.SetFloat("GeneralVolume", 0.6f);
            PlayerPrefs.SetFloat("EffectsVolume", 0.7f);
            PlayerPrefs.SetFloat("VoiceVolume", 0.8f);
            PlayerPrefs.SetFloat("MusicVolume", 0.6f);
        }
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs created");
    }

    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs deleted");
    }

    public void OpenAllLevels()
    {
        PlayerPrefs.SetString("CurrentProgress", "Ep2CargoStation");
    }
}
