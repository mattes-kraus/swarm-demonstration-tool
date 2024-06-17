using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionField : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField input;
    [SerializeField] private bool isX;
    [SerializeField] private TMP_Text xCaption;
    [SerializeField] private TMP_Text yCaption;

    void Start(){
        slider.onValueChanged.AddListener((value) => {
            this.ChangeValue(value);
        });

        input.onValueChanged.AddListener((value) => {
            float result;
            if (float.TryParse(value, out result)) {
                ChangeValue(result);
            } else {
                Debug.LogWarning("Input was not a float");
            }
        });
    }

    void Update(){
        if (GameManagement.selectedObject != null) {
            float value;

            // Check whether lightbulb is on x or y axis
            if (GameManagement.selectedObjectType == ObjectType.Lightbulb) {
                isX = Math.Abs(GameManagement.selectedObject.transform.position.z) == 2;
                xCaption.gameObject.SetActive(isX);
                yCaption.gameObject.SetActive(!isX);
            }

            // regular physical object logic
            if (isX){
                value = GameManagement.selectedObject.transform.position.x;  
            } else {
                value = GameManagement.selectedObject.transform.position.z;  
            }
            input.text = value.ToString();
            slider.value = value;
        }
    }

    private void ChangeValue(float value){
        if (GameManagement.selectedObject != null) {
            Transform selectedObjectTransform = GameManagement.selectedObject.transform;
            if(isX){
                selectedObjectTransform.position = new Vector3(value, selectedObjectTransform.position.y, selectedObjectTransform.position.z); // Neue Skalierung setzen
            } else {
                selectedObjectTransform.position = new Vector3(selectedObjectTransform.position.x, selectedObjectTransform.position.y, value); // Neue Skalierung setzen
            }
            slider.value = value;
            input.text = value.ToString();
        }
    }
}
