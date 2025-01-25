using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseDemo : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Sprite imageRun;
    [SerializeField] private Sprite imagePause;
    [SerializeField] private GameObject sprite;
    public void OnPointerDown(PointerEventData pointerEventData){
        SwitchGameState();
    }

    public void SwitchGameState(){
        if(GameManagement.gameState == GameState.Running){
            GameManagement.gameState = GameState.Paused;
            sprite.GetComponent<Image>().sprite = imageRun;

        } else {
            GameManagement.gameState = GameState.Running;
            sprite.GetComponent<Image>().sprite = imagePause;
        }
    }
}
