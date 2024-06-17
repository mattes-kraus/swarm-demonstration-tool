using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorField : MonoBehaviour
{
    [SerializeField] private Image displayColor;
    [SerializeField] private Slider slider;

    void Start(){
        slider.onValueChanged.AddListener((value) => {
            this.ChangeValue(value/255);
        });
        
    }

    void Update(){
        if (GameManagement.selectedObject != null) {
            float brightness =  GameManagement.selectedObject.gameObject.GetComponent<Renderer>().material.color.r; // zwischen 0 und 1 
            slider.value = brightness * 255;
            displayColor.color = new Color(brightness, brightness, brightness);
        }
    }

    private void ChangeValue(float brightness){
        if (GameManagement.selectedObject != null) {
            Color newColor = new Color(brightness, brightness, brightness);
            GameManagement.selectedObject.gameObject.GetComponent<Renderer>().material.color = newColor;
            // GameManagement.selectedObject.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
            // GameManagement.selectedObject.gameObject.GetComponent<Renderer>().UpdateGIMaterials();
        }
    }
}
