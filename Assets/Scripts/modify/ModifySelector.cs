using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    select the UI displayed to modify an object while customizing the arena
*/
public class ModifySelector : MonoBehaviour
{
    [SerializeField] private GameObject modifierGroundsticker, modifierWall, modifierSpawner, modifierLightbulb, modifyDelete;
    void Update()
    {
        modifyDelete.SetActive(GameManagement.selectedObjectType != ObjectType.Empty);
        modifierGroundsticker.SetActive(ObjectType.Groundsticker == GameManagement.selectedObjectType);
        modifierWall.SetActive(ObjectType.Wall == GameManagement.selectedObjectType);
        modifierSpawner.SetActive(ObjectType.Spawner == GameManagement.selectedObjectType);
        modifierLightbulb.SetActive(ObjectType.Lightbulb == GameManagement.selectedObjectType);
    }
}
