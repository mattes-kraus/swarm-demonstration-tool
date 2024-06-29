using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// button for changing the current behaviours
public class SetBehavior : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private BotBehavior newState;
    [SerializeField] private Image behaviorIndicator;
    [SerializeField] private bool behaviorIndicatorSet = false;

    void Start(){
        if (behaviorIndicatorSet) return;
        
        try{
            behaviorIndicator.color = GameManagement.GetColorByBehavior(newState);
            behaviorIndicatorSet = true;
        } catch (SystemException e){
            Debug.Log(e.Message);
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData){
        switch(GameManagement.currentControlMode)
        {
            // change the behavior of all selected bots to the new one
            case ControlMode.Selection:
                GameManagement.selectedBots.ForEach((bot) => {
                    bot.ChangeState(newState, GameManagement.selectedPos);
                });
                break;

            // prepare the behavior for newly added beacons
            case ControlMode.AddBeacon:
                GameManagement.selectedBehavior = newState;
                break;
            
            // switch selected beacon state
            case ControlMode.ModifyBeacon:
                if(GameManagement.selectedObject != null && GameManagement.selectedObjectType == ObjectType.Beacon){
                    GameManagement.selectedObject.GetComponent<Beacon>().behavior = newState;
                    GameManagement.selectedObject.GetComponent<Beacon>().targetPos = GameManagement.selectedPos;
                    GameManagement.DeselectCurrentObject();
                }            
                break;
            default: 
                break;
        }
    }
}
