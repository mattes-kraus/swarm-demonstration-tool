using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteField : MonoBehaviour,IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointerEventData){
        Destroy(GameManagement.selectedObject);
        GameManagement.DeselectCurrentObject();
    }
}
