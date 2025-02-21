using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class VisualiseCoverage : MonoBehaviour

{
    // communication with outside
    MetricManagement metricManager;
    public int index_x; 
    public int index_z; 
    
    
    // Visu
    int timeToVis = 0;
    [SerializeField] Renderer rend;
    TextMesh t;
    void Start()
    {
        metricManager = GameObject.Find("MetricManager").GetComponent<MetricManagement>();

        // gone too far
        if(index_x >= MetricManagement.N_GRIDS_X || index_z >= MetricManagement.N_GRIDS_Z) {
            Destroy(gameObject);
        } 

        // add text        
        t = transform.GetChild(0).gameObject.AddComponent<TextMesh>();
        t.fontSize = 7;
        t.transform.localEulerAngles += new Vector3(90, 0, 0);
        t.anchor = TextAnchor.MiddleCenter;
    }

    void Update()
    {
        // scale to right size
        float xscale = (GameManagement.ARENA_X_MAX - GameManagement.ARENA_X_MIN) / MetricManagement.N_GRIDS_X;
        float zscale = (GameManagement.ARENA_Z_MAX - GameManagement.ARENA_Z_MIN) / MetricManagement.N_GRIDS_Z;
        transform.localScale.Set(xscale, transform.localScale.y, zscale);

        // find out what part of the grid we are displaying and the value of it
        timeToVis = (int) metricManager.lastVisitTime[index_x, index_z];

        // become color dependant on time tile was never visited
        t.text = timeToVis.ToString();
        rend.material.SetColor("_EmissionColor", GetColorByTime(timeToVis));
        rend.material.EnableKeyword("_EMISSION");
        rend.UpdateGIMaterials();
    }

    Color GetColorByTime(float time){
        float end = 60;

        float r = (time/end);
        if(r > 1) r = 1;
        float g = 1 - r;

        return new Color(r,g,0);
    }
}
