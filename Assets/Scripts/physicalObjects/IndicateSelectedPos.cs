using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IndicateSelectedPos : MonoBehaviour
{
    // behaviour for the pin which shows the current selected position 
    void Update()
    {
        // make invisible as long we are building the arena 
        for (int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).GetComponent<Renderer>().enabled = !(GameManagement.gameState == GameState.Building);
        }

        // mark the current selected target position 
        transform.position = new Vector3(GameManagement.selectedPos.x, 2, GameManagement.selectedPos.z);
    }
}
