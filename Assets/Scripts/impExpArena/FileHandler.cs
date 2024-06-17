using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Text;
using UnityEditor.PackageManager;

[System.Serializable]
public class GameObjectData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Color color;
    public List<GameObjectData> children;
}
[System.Serializable]
public class BeaconData
{
    public Vector3 position;
    public BotBehavior behavior;
    public Vector3 behaviorTargetPosition;
    public Vector3 scale;
}
[System.Serializable]
public class ExportData
{
    public List<BeaconData> beacons;
}

public class FileHandler : MonoBehaviour
{
    public static void ExportGameObject(GameObject gameObject, string filePath)
    {
        GameObjectData gameObjectData = RecursivelyCreateGameObjectData(gameObject);
        string json = JsonUtility.ToJson(gameObjectData, true);
        File.WriteAllText(filePath, json);
    }

    public static void ExportBeacons(GameObject gameObject, string filePath)
    {
        ExportData exportData = new(){beacons = new()};
        
        for(int i=0; i<gameObject.transform.childCount; i++){
            GameObject curr = gameObject.transform.GetChild(i).gameObject; 

            BeaconData newBeaconData = new()
            {
                position = curr.transform.position,
                scale = curr.transform.localScale,
                behavior = curr.GetComponent<Beacon>().behavior,
                behaviorTargetPosition = curr.GetComponent<Beacon>().targetPos
            };
            newBeaconData.scale.y = 0.5f;
            newBeaconData.position.y = 0;
            exportData.beacons.Add(newBeaconData);
        }
        
        string json = JsonUtility.ToJson(exportData, true);
        File.WriteAllText(filePath, json);
    }

    public static void ImportGameObject(string filePath, GameObject parent)
    {
        ResetArena();
        try{
            string json = File.ReadAllText(filePath);
            GameObjectData gameObjectData = JsonUtility.FromJson<GameObjectData>(json);
            RecursivelyCreateGameObject(gameObjectData, parent);
        } catch {
            Debug.Log(filePath +  " not found");
        }
    }

    public static void ImportBeacons(string filePath, GameObject parent)
    {
        try{
            string json = File.ReadAllText(filePath);
            ExportData beaconData = JsonUtility.FromJson<ExportData>(json);
            RecursivelyCreateBeacons(beaconData.beacons, parent);
        } catch {
            Debug.Log(filePath +  " not found");
        }
    }

    private static GameObjectData RecursivelyCreateGameObjectData(GameObject gameObject)
    {
        // save all information the user could can change from original prefab
        GameObjectData gameObjectData = new GameObjectData
        {
            name = gameObject.name,
            position = gameObject.transform.localPosition,
            rotation = gameObject.transform.localRotation,
            scale = gameObject.transform.localScale
        };

        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null) gameObjectData.color = renderer.material.color; 

        // save all information of their childs
        int childCount = gameObject.transform.childCount;
        gameObjectData.children = new List<GameObjectData>();
        for (int i = 0; i < childCount; i++)
        {
            gameObjectData.children.Add(RecursivelyCreateGameObjectData(gameObject.transform.GetChild(i).gameObject));
        }

        return gameObjectData;
    }


    private static void RecursivelyCreateGameObject(GameObjectData gameObjectData, GameObject parent)
    {
        // init
        GameObject newGameObject;
        string prefabName = gameObjectData.name.Substring(0, gameObjectData.name.Length - 7); // schneide das "(Clone)" ab
        
        // Objekt wie im JSON spezifiziert erstellen
        GameObject prefab = Resources.Load<GameObject>("physical_prefabs/" + prefabName); // Lade das Prefab
        if (prefab != null)
        {
            newGameObject = GameObject.Instantiate(prefab, parent.transform); 
            newGameObject.transform.localPosition = gameObjectData.position;
            newGameObject.transform.localRotation = gameObjectData.rotation;
            newGameObject.transform.localScale = gameObjectData.scale;
            Renderer renderer = newGameObject.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.material.SetColor("_EmissionColor", gameObjectData.color);
                renderer.material.SetColor("_Color", gameObjectData.color);
                renderer.UpdateGIMaterials();
            } else {
                Debug.Log(newGameObject.name + "has no specified color");
            }
        // falls objekt kein prefab ist einfach mit children weiter machen und an ArenaModifications anhängen
        } else {
            newGameObject = GameObject.Find("ArenaModifications");
        }

        // repeat for all children
        if (gameObjectData.children != null)
        {
            foreach (var childData in gameObjectData.children)
            {
                RecursivelyCreateGameObject(childData, newGameObject);
            }
        }
    }

    private static void RecursivelyCreateBeacons(List<BeaconData> beaconDataList, GameObject parent)
    {
        // init
        GameObject newGameObject;
        string prefabName = "Beacon";
        
        // Objekt wie im JSON spezifiziert erstellen
        GameObject prefab = Resources.Load<GameObject>("physical_prefabs/" + prefabName); // Lade das Prefab
        if (prefab != null)
        {
            beaconDataList.ForEach((beaconData) => {
                newGameObject = GameObject.Instantiate(prefab, parent.transform); 
                newGameObject.transform.localPosition = beaconData.position;
                newGameObject.transform.localScale = beaconData.scale;
                newGameObject.GetComponent<Beacon>().behavior = beaconData.behavior;
                newGameObject.GetComponent<Beacon>().targetPos = beaconData.behaviorTargetPosition;
            });            
        // falls objekt kein prefab ist einfach mit children weiter machen und an ArenaModifications anhängen
        } else {
            Debug.Log("Couldnt find Beacon prefab");
        }
    }

    public static void ResetArena(){
        // destroy arena mods like groundsticker, walls,...
        GameObject mods = GameObject.Find("ArenaModifications");
        int childCount = mods.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(mods.transform.GetChild(i).gameObject);
        }

        // destroy beaconcs
        mods = GameObject.Find("Beacons");
        childCount = mods.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(mods.transform.GetChild(i).gameObject);
        }

        // destroy beaconcs
        mods = GameObject.Find("Robots");
        childCount = mods.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(mods.transform.GetChild(i).gameObject);
        }
        
        GameObject.Find("MetricManager").GetComponent<MetricManagement>().allBots.Clear();
    }
}



