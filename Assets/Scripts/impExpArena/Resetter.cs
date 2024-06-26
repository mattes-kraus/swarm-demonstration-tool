using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Resetter : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointerEventData){
        FileHandler.ResetArena();
    }
}
