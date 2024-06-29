using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModificationAdder : MonoBehaviour
{
    [SerializeField] private GameObject wall, spawnpoint, sticker, beacon;
    [SerializeField] private Transform  modificationParent;
    [SerializeField] private Transform  beaconParent;
    [SerializeField] private Image      selectTargetPos;
    // [SerializeField] private Texture2D cursorTextureTarget;

    void OnMouseDown()
    {
        // localize mouseposition
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo);

        // deselect the current selected element 
        GameManagement.DeselectCurrentObject();

        SetBehaviorPos(hitInfo.point.x, hitInfo.point.z);
        AddNewElement(hitInfo.point);
    }

    public void SetBehaviorPos(float x, float z){
        // check if we are currently selecting a position for behavior changes
        if(GameManagement.targetLocSelectActive){
            GameObject.Find("position_click").transform.GetChild(0).GetComponent<TMP_InputField>().text = "("+x+"/"+z+")"; 
            GameManagement.targetLocSelectActive = false;
            selectTargetPos.color = Color.white;
        }
    }

    public void AddNewElement(Vector3 pos){
        // Instantiate the GameObject at the position of the hit
        switch (GameManagement.newElementType)
        {
            case ObjectType.Groundsticker:
                Instantiate(sticker, pos, Quaternion.identity, modificationParent);
                break;
            case ObjectType.Wall:
                Instantiate(wall, pos, Quaternion.identity, modificationParent);
                break;
            case ObjectType.Spawner:
                Instantiate(spawnpoint, pos, Quaternion.identity, modificationParent);
                break;
            case ObjectType.Beacon:
                if(GameManagement.currentControlMode == ControlMode.AddBeacon){
                    GameObject instBeacon = Instantiate(beacon, pos, Quaternion.identity, beaconParent);
                    instBeacon.GetComponent<Beacon>().behavior = GameManagement.selectedBehavior;
                    instBeacon.GetComponent<Beacon>().targetPos = GameManagement.selectedPos;
                }
                break;
            default:
                break;
        }
    }
}
