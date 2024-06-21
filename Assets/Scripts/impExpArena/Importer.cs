using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Importer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject modParent;
    [SerializeField] private GameObject beaconParent;
    [SerializeField] private TMP_InputField input;

    public void OnPointerDown(PointerEventData pointerEventData){
        string path = "./Saved_Arenas/" + input.text + ".json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ImportGameObject(path, modParent);
        path = "./Saved_Arenas/" + input.text + "_beacons.json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ImportBeacons(path, beaconParent);
    }
}
