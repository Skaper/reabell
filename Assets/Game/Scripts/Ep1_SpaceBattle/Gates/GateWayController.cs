using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateWayController : MonoBehaviour
{
    public GameObject pointerObject;
    private Battlehub.SplineEditor.PointerSplineMover pointer;

    public Transform gate1;
    public Transform gate2;
    public Transform gate3;
    public Transform gate4;
    public Battlehub.SplineEditor.SplineBase Spline0_1;
    public Battlehub.SplineEditor.SplineBase Spline1_2;
    public Battlehub.SplineEditor.SplineBase Spline2_3;
    public Battlehub.SplineEditor.SplineBase Spline3_4;

    void Start()
    {
        pointer = pointerObject.GetComponent<Battlehub.SplineEditor.PointerSplineMover>();
        pointerObject.SetActive(false);
        pointer.Spline = Spline0_1;
        pointer.IsRunning = true;
        pointer.IsLoop = true;
        GameManager.QuestManagerEp1.onActionPlayerShipActive += ActionPlayerInShip;
        GameManager.QuestManagerEp1.onActionGatesPassed += ActionGatesPassed;
        GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(gate1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Активация поинтера
    private void ActionPlayerInShip()
    {
        pointerObject.SetActive(true);
        GameManager.QuestManagerEp1.onActionPlayerShipActive -= ActionPlayerInShip;
    }

    private void ActionGatesPassed(int gateNumber)
    {
        Debug.Log("Gate passed num.: " + gateNumber);
        switch (gateNumber)
        {
            case 1:
                pointer.enabled = true;
                pointer.Spline = Spline1_2;
                pointer.IsRunning = true;
                GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(gate2);
                break;
            case 2:
                pointer.enabled = true;
                pointer.Spline = Spline2_3;
                pointer.IsRunning = true;
                GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(gate3);
                break;
            case 3:
                pointer.enabled = true;
                pointer.Spline = Spline3_4;
                pointer.IsRunning = true;
                GameManager.QuestManagerEp1.onActionChangeTarget?.Invoke(gate4);
                break;
            case 4:
                pointer.enabled = false;
                enabled = false;
                GameManager.QuestManagerEp1.onActionGatesPassed -= ActionGatesPassed;
                break;
        }
    }
}
