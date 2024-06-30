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

    // import an arena configuration from a json file
    public void OnPointerDown(PointerEventData pointerEventData){
        Debug.Log("." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + input.text + ".json");
        string path = "." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + input.text + ".json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ImportGameObject(path, modParent);
        path = "." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + input.text + "_beacons.json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ImportBeacons(path, beaconParent);
    }
}
