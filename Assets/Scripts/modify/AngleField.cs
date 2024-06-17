using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AngleField : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField input;

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
        if(GameManagement.selectedObject != null){
            slider.value = GameManagement.selectedObject.transform.eulerAngles.y;
            input.text = slider.value.ToString();
        }
    }

    private void ChangeValue(float value){
        if (GameManagement.selectedObject != null) {
            Transform selectedObjectTransform = GameManagement.selectedObject.transform;

            // selectedObjectTransform.rotation = Quaternion.identity;
            selectedObjectTransform.rotation = Quaternion.Euler(0f, value, 0f);; // Neuen Winkel setzen
        }
        input.text = value.ToString();
    }
}
