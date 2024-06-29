using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutterWall : MonoBehaviour
{
    [SerializeField] private GameObject lightbulb;
    [SerializeField] private Transform parent;

    public void OnMouseDown(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // Check whether the raycast hits a collider and whether lightbulb is selected
        if (Physics.Raycast(ray, out hitInfo) && GameManagement.newElementType == ObjectType.Lightbulb)
        {
            Instantiate(lightbulb, hitInfo.point, Quaternion.identity, parent);
        }
    }
}
