using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnZoneField : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField input;

    void Start(){
        slider.onValueChanged.AddListener((value) => {
            this.ChangeValue((int) value);
        });

        input.onValueChanged.AddListener((value) => {
            int result;
            if (int.TryParse(value, out result)) {
                ChangeValue(result);
            } else {
                Debug.LogWarning("Input was not a int");
            }
        });
    }

    void Update(){
        if (GameManagement.selectedObject != null) {
            input.text = GameManagement.selectedObject.GetComponent<SpawnZone>().robotsToSpawn.ToString();
            slider.value = GameManagement.selectedObject.GetComponent<SpawnZone>().robotsToSpawn;
        }
    }

    private void ChangeValue(int value){
        if (GameManagement.selectedObject != null) {
            SpawnZone selectedObjectTransform = GameManagement.selectedObject.GetComponent<SpawnZone>();
            selectedObjectTransform.robotsToSpawn = value; // Neue Skalierung setzen
            slider.value = value;
            input.text = value.ToString();
        }
    }
}
