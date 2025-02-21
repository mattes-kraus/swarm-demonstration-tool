using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Leave the arena customization and start the actual demonstration
public class StartDemo : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject demoUI;
    [SerializeField] private Transform parent;
    public void OnPointerDown(PointerEventData pointerEventData){
        GameManagement.gameState = GameState.Paused;
        GameManagement.ChangeNewElementType(ObjectType.Beacon, null);
        try{
            demoUI.SetActive(true);
            GameObject.Find("BuildUI").SetActive(false);
        } catch(SystemException){
            Debug.Log("can't find BuildUI");
        }
    }
}
