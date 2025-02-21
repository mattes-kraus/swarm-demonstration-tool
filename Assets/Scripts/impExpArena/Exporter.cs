using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


// export the arena configuration to a json file
public class Exporter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject modificationsToSave;
    [SerializeField] private GameObject beaconsToSave;
    [SerializeField] private TMP_InputField input;

    // export the arena configuration to a json file
    public void OnPointerDown(PointerEventData pointerEventData){
        string path = "." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + input.text + ".json";
        try{
            Directory.CreateDirectory("." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar);
        } catch(SystemException){
            // it's alright, then the directory already exists
        }

        if (!string.IsNullOrEmpty(input.text)) FileHandler.ExportGameObject(modificationsToSave, path);
        
        path = "." + Path.DirectorySeparatorChar + "SavedArenas" + Path.DirectorySeparatorChar + input.text + "_beacons.json";
        if (!string.IsNullOrEmpty(input.text)) FileHandler.ExportBeacons(beaconsToSave, path);
    }
}