using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ObjectType newSpawnType;
    [SerializeField] private Image background;
    

    void Start(){
        GameManagement.addButtonBackgrounds.Add(background);
    }

    // Update is called once per frame
    public void OnPointerDown(PointerEventData pointerEventData){
        GameManagement.ChangeNewElementType(newSpawnType, background);
    }
}
