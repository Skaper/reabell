using System;
using UnityEngine;

public class QuestManagerEp2 : MonoBehaviour
{

    public Action OnAction_LevelStart;

    public Action<int> OnActionTowerCrane_BatteryAdd;
    public Action OnActionTowerCrane_Grab;
    public Action OnActionShip_came;
    public Action OnActionControlPanel_access;

    public Action OnActionAirCompressor_off;

    public Action OnActionCallTerminall_on;


    public Action OnActionMissionFailed;
    public Action OnActionTowerCrane_ChargingComplited;

    public Action OnActionPlayerInsideShip;

    public Action OnActionDead;

    public bool levelStart;

    public int towerCrane_batteryCount = 0;
    public bool ship_come = false;
    public bool towerCrane_grabDown = false;
    public bool controlPanel_access = false;
    public bool airCompressor = true;
    public bool callTerminall = false;
    public bool chargingComlited = false;

    public bool isPlayerDead = false;
    
    public static float ChargingTime { get; [SerializeField] private set; } = 60f;
    public static float LevelStartDelay { get; private set; } = 10f;
    private float levelStartTimer;
    private void Awake()
    {
        GameManager.QuestManagerEp2 = this;
        OnAction_LevelStart += QuestManager_OnAction_LevelStart;

        OnActionTowerCrane_BatteryAdd += QuestManager_OnActionTowerCrane_BatteryAdd;
        OnActionShip_came += QuestManager_OnActionShip_came;
        OnActionTowerCrane_Grab += QuestManager_OnActionTowerCrane_grab;
        OnActionControlPanel_access += QuestManager_OnActionControlPanel_access;
        OnActionAirCompressor_off += QuestManager_OnActionCompressor_off;
        OnActionCallTerminall_on += QuestManager_OnActionCallTerminall_on;
        OnActionTowerCrane_ChargingComplited += QuestManager_OnActionTowerCrane_ChargingComplited;
        OnActionDead = QuestManager_OnActionDead;
    }

    private void QuestManager_OnActionTowerCrane_BatteryAdd(int count)
    {
        towerCrane_batteryCount = count;
    }

    private void QuestManager_OnActionShip_came()
    {
        ship_come = true;
    }

    private void QuestManager_OnActionTowerCrane_grab()
    {
        towerCrane_grabDown = true;
    }

    private void QuestManager_OnActionControlPanel_access()
    {
        controlPanel_access = false;
    }

    private void QuestManager_OnActionCompressor_off()
    {
        airCompressor = false;
    }

    private void QuestManager_OnActionCallTerminall_on()
    {
        callTerminall = true;
    }

    private void QuestManager_OnActionTowerCrane_ChargingComplited()
    {
        chargingComlited = true;
    }

    private void QuestManager_OnAction_LevelStart()
    {
        levelStart = true;
    }

    private void QuestManager_OnActionDead()
    {
        isPlayerDead = true;
    }

    void Update()
    {
        if (!levelStart) {
            levelStartTimer += Time.deltaTime;
            if(levelStartTimer >= LevelStartDelay) OnAction_LevelStart?.Invoke();
        }
    }
}
