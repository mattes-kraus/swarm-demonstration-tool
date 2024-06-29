using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickPosition : MonoBehaviour,IPointerDownHandler
{
    // if you click this button, you can select a new position to then later 
    // use to modify a beahviour of a robot 
    [SerializeField] private Image image;
    public void OnPointerDown(PointerEventData pointerEventData){
        GameManagement.targetLocSelectActive = !GameManagement.targetLocSelectActive; 
        if(GameManagement.targetLocSelectActive){
            image.color = Color.green;
        } else {
            image.color = Color.white;
        }
    }
}