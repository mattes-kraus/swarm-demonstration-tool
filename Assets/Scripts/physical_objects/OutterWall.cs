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

        // Überprüfe, ob der Raycast ein Collider trifft und ob lightbulb ausgewählt ist
        if (Physics.Raycast(ray, out hitInfo) && GameManagement.newElementType == ObjectType.Lightbulb)
        {
            Instantiate(lightbulb, hitInfo.point, Quaternion.identity, parent);
        }
    }
}
