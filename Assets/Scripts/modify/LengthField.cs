using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LengthField : MonoBehaviour
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
            slider.value = GameManagement.selectedObject.transform.localScale.x;
            input.text = slider.value.ToString();
        }
    }

    private void ChangeValue(float value){
        if (GameManagement.selectedObject != null) {
            Vector3 scale = GameManagement.selectedObject.transform.localScale;
            GameManagement.selectedObject.transform.localScale = new Vector3(value, scale.y, scale.z); // Neue Skalierung setzen
            slider.value = value;
            input.text = value.ToString();
        }
    }
}
