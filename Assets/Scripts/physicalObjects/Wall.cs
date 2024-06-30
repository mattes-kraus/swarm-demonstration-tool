using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    void OnDestroy(){
        VoronoiDiagram.walls.Remove(gameObject);
    }
}
