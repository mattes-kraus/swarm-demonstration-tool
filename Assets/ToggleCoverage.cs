using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCoverage : MonoBehaviour
{
    [SerializeField] Toggle toggle;

    void Start(){
        toggle.onValueChanged.AddListener((bool value) => ToggleCoverageMetric(value));
    }
    
    void ToggleCoverageMetric(bool show){
        GameObject metricManager = GameObject.Find("MetricManager");
        if(show){
            // show metric
            metricManager.GetComponent<MetricManagement>().ShowCoverage();
        } else {
            // delete all metric grid tiles
            int childCount = metricManager.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(metricManager.transform.GetChild(i).gameObject);
            }
        }
    }
}
