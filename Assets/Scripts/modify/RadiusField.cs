using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadiusField : MonoBehaviour
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
        if (GameManagement.selectedObject != null) {
            input.text = GameManagement.selectedObject.transform.localScale.x.ToString();
            slider.value = GameManagement.selectedObject.transform.localScale.x;
        }
    }

    private void ChangeValue(float value){
        if (GameManagement.selectedObject != null) {
            Transform selectedObjectTransform = GameManagement.selectedObject.transform;
            selectedObjectTransform.localScale = new Vector3(value, selectedObjectTransform.localScale.y, value);
            slider.value = value;
            input.text = value.ToString();
        }
    }
}
