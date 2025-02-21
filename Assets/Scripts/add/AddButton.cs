using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// When clicked, the user is able to add elements when clicking on the arena
public class AddButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ObjectType newSpawnType;
    [SerializeField] private Image background;
    

    void Start(){
        GameManagement.addButtonBackgrounds.Add(background);
    }

    public void OnPointerDown(PointerEventData pointerEventData){
        GameManagement.ChangeNewElementType(newSpawnType, background);
    }
}
