using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerEp1 : MonoBehaviour
{
    public AIHelperEp1 AIHelper;
    // Активация коробля
    public Action onActionPlayerShipActive; //action 1 +
    // Прохождение через врата 1-4
    public Action<int> onActionGatesPassed; //action 2 +
    // Уничтожение маячка
    public Action onActionBeaconDestroyed;  //action 3 +
    // Враги активировались
    public Action onActionEnemiesAwakened;  //action 4 + 
    // Враги уничтожены
    public Action onActionEnemiesDestroyed; //action 5 

    public Action onActionDeadShipScanned;  //action 6

    public Action onActionColossalShipFound;//action 7

    public Action onActionEndOfLevel;       //action 8

    public Action<Transform> onActionChangeTarget;

    public bool PlayerShipActive { get; private set; }
    public int GatePassedNumber { get; private set; }
    public bool BeaconDestroyed { get; private set; }
    public bool EnemiesAwakened { get; private set; }
    public bool EnemiesDestroyed { get; private set; }
    public bool DeadShipScanned { get; private set; }
    public bool ColossalShipFound { get; private set; }

    [Header("Debug tools")]
    public bool debug_PlayerShipActive = false;
    private bool d_playerShipActive = false;

    public bool debug_GatesPassed = false;
    private bool d_gatePassed = false;

    public bool debug_BeaconDestroyed = false;
    private bool d_beaconDestroyed = false;

    public bool debug_EnemiesAwakened = false;
    private bool d_enemiesAwakened = false;

    public bool debug_EnemiesDestroyed= false;
    private bool d_enemiesDestroyed = false;

    public bool debug_DeadShipScanned = false;
    private bool d_deadShipScanned = false;

    public bool debug_ColossalShipFound = false;
    private bool d_colossalShipFound = false;

    [Header("Enemes on level")]
    public List<GameObject> enemes;

    

    private void Awake()
    {
        GC.Collect();

        GameManager.QuestManagerEp1 = this;
        onActionPlayerShipActive += actionPlayerShipActive;
        onActionGatesPassed += actionGatesPassed;
        onActionBeaconDestroyed += actionBeaconDestroyed;
        onActionEnemiesAwakened += actionEnemiesAwake;
        onActionEnemiesDestroyed += actionEnemiesDestroyed;
        onActionDeadShipScanned += actionDeadShipScanned;
        onActionColossalShipFound += actionColossalShipFound;
    }

    private void Update()
    {
        if (debug_GatesPassed && !d_gatePassed)
        {
            onActionGatesPassed?.Invoke(4);
            d_gatePassed = true;
        }
        if (debug_BeaconDestroyed && !d_beaconDestroyed)
        {
            onActionBeaconDestroyed?.Invoke();
            d_beaconDestroyed = true;
        }
        if(debug_PlayerShipActive && !d_playerShipActive)
        {
            onActionPlayerShipActive?.Invoke();
            d_playerShipActive = true;
        }

        if (debug_DeadShipScanned && !d_deadShipScanned)
        {
            onActionDeadShipScanned?.Invoke();
            d_deadShipScanned = true;
        }

        if (debug_EnemiesAwakened && !d_enemiesAwakened)
        {
            onActionEnemiesAwakened?.Invoke();
            d_enemiesAwakened = true;
        }

        if (debug_EnemiesDestroyed && !d_enemiesDestroyed)
        {
            onActionEnemiesDestroyed?.Invoke();
            d_enemiesDestroyed = true;
        }

        if (debug_ColossalShipFound && !d_colossalShipFound)
        {
            onActionColossalShipFound?.Invoke();
            d_colossalShipFound = true;
        }

    }

    private void actionPlayerShipActive()
    {
        debug_PlayerShipActive = true;
        PlayerShipActive = true;
    }

    private void actionGatesPassed(int gateNumber)
    {
        if (gateNumber == 4)
        {
            d_gatePassed = true;
            debug_GatesPassed = true;
        }
        GatePassedNumber = gateNumber;
    }

    private void actionBeaconDestroyed()
    {
        d_beaconDestroyed = true;
        debug_BeaconDestroyed = true;
        BeaconDestroyed = true;
    }

    private void actionEnemiesAwake()
    {
        debug_EnemiesAwakened = true;
        d_enemiesAwakened = true;
        EnemiesAwakened = true;
    }

    private void actionEnemiesDestroyed()
    {
        debug_EnemiesDestroyed = true;
        d_enemiesDestroyed = true;
        EnemiesDestroyed = true;
    }

    private void actionDeadShipScanned()
    {
        debug_DeadShipScanned = true;
        d_deadShipScanned = true;
        DeadShipScanned = true;
    }

    private void actionColossalShipFound()
    {
        debug_ColossalShipFound = true;
        d_colossalShipFound = true;
        ColossalShipFound = true;
    }
}
