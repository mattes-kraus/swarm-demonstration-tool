using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


// import an arena configuration from a json file
public class Importer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject modParent;
    [SerializeField] private GameObject beaconParent;
    [SerializeField] private TMP_InputField input;

    // import an arena configuration from a json file
    public void OnPointerDown(PointerEventData pointerEventData){
        LoadArena(input.text);
    }

    public void LoadArena(string name){
        Debug.Log("." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + name + ".json");
        string path = "." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + name + ".json";
        if (!string.IsNullOrEmpty(name)) FileHandler.ImportGameObject(path, modParent);
        path = "." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + name + "_beacons.json";
        if (!string.IsNullOrEmpty(name)) FileHandler.ImportBeacons(path, beaconParent);
    }
}
