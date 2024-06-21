using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Exporter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject modificationsToSave;
    [SerializeField] private GameObject beaconsToSave;
    [SerializeField] private TMP_InputField input;
    public void OnPointerDown(PointerEventData pointerEventData){
        string path = "./Saved_Arenas/" + input.text + ".json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ExportGameObject(modificationsToSave, path);
        
        path = "./Saved_Arenas/" + input.text + "_beacons.json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ExportBeacons(beaconsToSave, path);
    }
}