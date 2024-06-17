using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlModeSelector : MonoBehaviour
{
    [SerializeField] private GameObject beacon;
    [SerializeField] private GameObject modifyButtons;
    public void ChangeControlMode(Int32 newMode)
    {
        switch (newMode)
        {
            case 0:
                GameManagement.currentControlMode = ControlMode.Selection;
                GameManagement.StopAdding();
                modifyButtons.SetActive(false);
                break;
            case 1:
                GameManagement.currentControlMode = ControlMode.AddBeacon;
                GameManagement.ChangeNewElementType(ObjectType.Beacon, null);
                modifyButtons.SetActive(false);
                break;
            case 2:
                GameManagement.currentControlMode = ControlMode.ModifyBeacon;
                GameManagement.StopAdding();
                modifyButtons.SetActive(true);
                break;
        }
    }
}
