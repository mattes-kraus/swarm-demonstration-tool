using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    void OnDestroy(){
        GameObject.Find("VoronoiVis").GetComponent<VoronoiDiagram>().walls.Remove(gameObject);
    }
}
