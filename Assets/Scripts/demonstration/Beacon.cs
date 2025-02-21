using System;
using UnityEngine;


// Beacon is a game object that can be placed in the arena to change
// behavior of the turtlebots that collide with it
public class Beacon : MonoBehaviour
{
    public BotBehavior behavior = BotBehavior.Random;
    public Vector3 targetPos = Vector3.zero;

    void Update(){
        Color color;
        if(GameManagement.selectedObject == gameObject){
            color = Color.green;
        } else {
            color = GameManagement.GetColorByBehavior(behavior);
        }

        GetComponent<Renderer>().material.SetColor("_EmissionColor", color);    
        GetComponent<Renderer>().UpdateGIMaterials();
    }

    void OnTriggerEnter(Collider collider){
        try{
            collider.gameObject.GetComponent<Turtlebot>().ChangeState(behavior, targetPos);
        } catch (SystemException){
            // not a turtlebot
        }
    }
}
