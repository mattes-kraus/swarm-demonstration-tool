using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StopAdding : MonoBehaviour, IPointerDownHandler
{
    // button in GUI. problem is, once you clicked an element to add (lightbulb, etc.), the next click on the 
    // plane will place it. If you don't want this, click this button.
    public void OnPointerDown(PointerEventData pointerEventData){
        GameManagement.StopAdding();
    }
}
