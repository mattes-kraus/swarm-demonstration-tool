using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameManagement
{
    /* ------ manage game state -------------------------------------- */
    public static GameState gameState = GameState.Building;

    /* ------ manage demonstrating behavior -------------------------- */
    // current control mode while demonstrating
    public static ControlMode currentControlMode = ControlMode.Selection;
    // list of selected bots which bahviors will be changed on behavior change click
    public static List<Turtlebot> selectedBots = new List<Turtlebot>();
    // position for behaviors like Come, Leave, ...
    public static Vector3 selectedPos = Vector3.zero;
    // position clicker active or not
    public static bool targetLocSelectActive = false;
    // behavior for the beacons
    public static BotBehavior selectedBehavior = BotBehavior.Random;

    /* ------ manage building the arena ------------------------------ */
    // new objects
    public static ObjectType newElementType;    
    
    // existing objects
    public static GameObject selectedObject;
    public static ObjectType selectedObjectType;

    // manage add-box
    public static List<Image> addButtonBackgrounds = new List<Image>();

    /* ------- arena constants --------------------------------------- */
    public const int ARENA_X_MIN = -2;
    public const int ARENA_X_MAX = 2;
    public const int ARENA_Z_MIN = -2;
    public const int ARENA_Z_MAX = 2;

    /* ---------- nach außensichtbare Mehtoden ----------------------- */
    public static void ChangeSelectedObject(GameObject gameObject, ObjectType type){
        DeselectCurrentObject();

        try {
            Beacon beacon = gameObject.GetComponent<Beacon>();
            GameManagement.selectedPos = beacon.targetPos;
        } catch (System.Exception){}

        // set new selected object
        selectedObject = gameObject;
        selectedObjectType = type;
        selectedObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        selectedObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
        selectedObject.GetComponent<Renderer>().UpdateGIMaterials();
    }

    public static void StopAdding(){
        DeselectNewObject();
        newElementType = ObjectType.Empty;
    }

    public static void ChangeNewElementType(ObjectType ElementType, Image backgroundToColor){
        DeselectCurrentObject();
        DeselectNewObject();
        newElementType = ElementType;
        try{
            backgroundToColor.color = Color.green;
        }catch (SystemException){}
    }
    public static void DeselectCurrentObject(){
        selectedObjectType = ObjectType.Empty;
        if(selectedObject != null){
            Material material = selectedObject.gameObject.GetComponent<Renderer>().material;
            material.SetColor("_EmissionColor", material.color);
            selectedObject = null;
        }
    }

    public static Color GetColorByBehavior(BotBehavior behavior){
        switch(behavior)
            {
                case BotBehavior.Stop:
                    return new Color(132f/255, 98f/255, 103f/255);
                case BotBehavior.Come:
                    return new Color(135f/255, 168f/255, 120f/255);
                case BotBehavior.Leave:
                    return new Color(59f/255, 96f/255, 100f/255);
                case BotBehavior.Random:
                    return new Color(239f/255,189f/255,235f/255);

                default:
                    return Color.black;
            }
    }

    /* ---------- Utilities ------------------------------------------ */
    private static void DeselectNewObject(){
        foreach(Image background in addButtonBackgrounds){
            background.color = Color.white;
        }
    }

}

public enum ObjectType {
    Empty,
    Groundsticker,
    Wall,
    Spawner,
    Lightbulb,
    Beacon
}

public enum GameState {
    Building,
    Running,
    Evaluation,
    Paused
}

public enum ControlMode {
    Selection,
    AddBeacon,
    ModifyBeacon
}