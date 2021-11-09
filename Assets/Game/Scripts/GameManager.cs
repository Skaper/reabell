using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Invector;
public static class GameManager
{

    //Main
    public static QuestManagerEp1 QuestManagerEp1;
    public static QuestManagerEp2 QuestManagerEp2; 
    public static JoystickControllSetup joystickControll;

    public static GameObject AIHelper;

    //Events
    public static Action<Transform> OpenMenu;

    //Player
    public static GameObject WorldCenterPoint { set; get; }
    public static GameObject Player { set; get; }

    public static PlayerReferences PlayerReferences { set; get; }
    public static GameObject PlayerCamera { set; get; }
    public static GameObject PlayerSpawnPoint { set; get; }

    //GUI
    public static MenuSystem MenuSystem { set; get; }
    public static GameObject PlayerMenuPoint { set; get; }


    public static GameObject PlayerHologrammPoint { set; get; }

    //Contsants
    public static readonly float LevelRadius = 50f;

}
