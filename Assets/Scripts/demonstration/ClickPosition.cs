using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickPosition : MonoBehaviour,IPointerDownHandler
{
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