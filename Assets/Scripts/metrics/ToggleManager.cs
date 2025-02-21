using System.Collections;
using System.Collections.Generic;
using csDelaunay;
using UnityEngine;
using UnityEngine.UI;


// Manage coverage and voronoi visualization
public class ToggleManager : MonoBehaviour
{    
    public void ToggleCoverageMetric(bool show){
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

    public void ToggleVoronoiVisu(bool enableVornoi){
        VoronoiDiagram.visuOn = enableVornoi;
    }
}
