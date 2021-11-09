using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AIHelper : MonoBehaviour
{
    public int MaxClipQueueLength = 2;

    [System.Serializable]
    public class Speech
    {
        public string name;
        public AudioClip clip;
        public bool isPlayed = false;
        public bool playManyTimes = false;
    }
    public QuestLoggerEp2 questLogger;

    public Light hologramLight;
    public ParticleSystem FXHologram;
    public GameObject batteryHologramm;
    private bool isBatteryHologrammShown = false;
    public GameObject idScannerHologramm;
    private bool isScannerHologrammShown = false;
    public GameObject oxygenTankHologramm;
    private bool isOxygenTankHologrammShown = false;

    private bool audioHasPlayed = false;

    public float delayOnStart;
    private float timer;
    private bool startPlaying = false;

    public string[] playOnStart;

    public Speech[] speech;

    private AudioSource audioSource;

    private List<Speech> clipsQueue = new List<Speech>() { };

    private void Awake()
    {
        //Init helper
        GameManager.AIHelper = gameObject;
    }
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        
        GameManager.QuestManagerEp2.OnActionShip_came += OnActionShip_came;
        GameManager.QuestManagerEp2.OnActionTowerCrane_Grab += OnActionTowerCrane_Grab;
        GameManager.QuestManagerEp2.OnAction_LevelStart += OnActionLevelStart;


        SetupHologramms();
    }

    private void OnActionLevelStart()
    {
        foreach (string name in playOnStart)
        {
            Play(name);
        }

        GameManager.QuestManagerEp2.OnAction_LevelStart -= OnActionLevelStart;
    }

    private void OnActionShip_came()
    {
        Play("ai_e7");
        GameManager.QuestManagerEp2.OnActionShip_came -= OnActionShip_came;
    }

    private void OnActionTowerCrane_Grab()
    {
        Play("ai_e8");
        GameManager.QuestManagerEp2.OnActionTowerCrane_Grab -= OnActionTowerCrane_Grab;
    }

    private int currentClipIndex = -1;
    private string currentClipName ="";
    // Update is called once per frame
    void Update()
    {
        if (startPlaying)
        {
            ShowHologram();
            if (!audioSource.isPlaying)
            {
                if (clipsQueue.Count > MaxClipQueueLength) clipsQueue.RemoveAt(0);
                if (clipsQueue.Count > 0)
                {
                    int index = clipsQueue.Count - 1;
                    audioSource.clip = clipsQueue[index].clip;
                    audioSource.Play();
                    clipsQueue[index].isPlayed = clipsQueue[index].playManyTimes ? false : true;
                    currentClipName = clipsQueue[index].name;
                    currentClipIndex = index;
                    audioHasPlayed = true;
                }
            }
            else
            {
                if (currentClipIndex >= 0)
                {
                    clipsQueue.Remove(clipsQueue[currentClipIndex]);
                    currentClipIndex = -1;
                }

            }
            if (!audioSource.isPlaying && audioHasPlayed)
            {
                audioHasPlayed = false;
                currentClipName = "";
            }



        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= delayOnStart) startPlaying = true;
        }

        
    }

    private void SetupHologramms()
    {
        FXHologram.Stop();
        hologramLight.enabled = false;
        Vector3 scale = batteryHologramm.transform.localScale;
        batteryHologramm.transform.SetParent(GameManager.PlayerHologrammPoint.transform);
        batteryHologramm.transform.position = GameManager.PlayerHologrammPoint.transform.position;
        batteryHologramm.transform.localScale = scale;

        scale = idScannerHologramm.transform.localScale;
        idScannerHologramm.transform.SetParent(GameManager.PlayerHologrammPoint.transform);
        idScannerHologramm.transform.position = GameManager.PlayerHologrammPoint.transform.position;
        idScannerHologramm.transform.localScale = scale;

        scale = oxygenTankHologramm.transform.localScale;
        oxygenTankHologramm.transform.SetParent(GameManager.PlayerHologrammPoint.transform);
        oxygenTankHologramm.transform.position = GameManager.PlayerHologrammPoint.transform.position;
        oxygenTankHologramm.transform.localScale = scale;
    }

    public void ShowHologram()
    {
        
        if (currentClipName.Equals("ai_m1"))
        {
            if(!FXHologram.isPlaying) FXHologram.Play();
            hologramLight.enabled = true ;
            batteryHologramm.SetActive(true);
            isBatteryHologrammShown = true;
        }
        else if (isBatteryHologrammShown)
        {
            if (FXHologram.isPlaying) FXHologram.Stop();
            batteryHologramm.transform.SetParent(null);
            batteryHologramm.SetActive(false);
            isBatteryHologrammShown = false;
            hologramLight.enabled = false;
        }

        if (currentClipName.Equals("ai_e3"))
        {
            if (!FXHologram.isPlaying) FXHologram.Play();
            idScannerHologramm.SetActive(true);
            isScannerHologrammShown = true;
            hologramLight.enabled = true;
        }
        else if (isScannerHologrammShown)
        {
            if (FXHologram.isPlaying) FXHologram.Stop();
            idScannerHologramm.transform.SetParent(null);
            idScannerHologramm.SetActive(false);
            isScannerHologrammShown = false;
            hologramLight.enabled = false;
        }

        if (currentClipName.Equals("ai_e3_3"))
        {
            if (!FXHologram.isPlaying) FXHologram.Play();
            oxygenTankHologramm.SetActive(true);
            isOxygenTankHologrammShown = true;
            hologramLight.enabled = true;
        }
        else if (isOxygenTankHologrammShown)
        {
            if (FXHologram.isPlaying) FXHologram.Stop();
            oxygenTankHologramm.transform.SetParent(null);
            oxygenTankHologramm.SetActive(false);
            isOxygenTankHologrammShown = false;
            hologramLight.enabled = false;
        }

    }

    public void Play(string clipName)
    {
        if (isClipInQueue(clipName)) return;
        Speech target = getClip(clipName);
        if (target == null)
        {
            Debug.LogError("Clip \"" + clipName + "\" cannot be found!");
            return;
        }
        if (!target.isPlayed || target.playManyTimes)
            clipsQueue.Add(target);
        questLogger.MakeLog(clipName);
    }

    
    private Speech getClip(string name)
    {
        foreach (Speech item in speech)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
        }
        return null;
    }

    private bool isClipInQueue(string clipName)
    {
        foreach (Speech item in clipsQueue)
        {
            if (item.name == clipName)
            {
                return true;
            }
        }
        return false;
    }
}
