using csDelaunay;
using UnityEngine;

/** idea is to call updates of static classes. Currently only used for voronoi,
    can be exetended further */
public class UpdateCaller : MonoBehaviour{
    // --- timestep management -------------
    private const float UPDATE_STEP = 2f;
    private float pastTime = 0f;
    [SerializeField] private GameObject siteMarker;
    
    void Start(){
        VoronoiDiagram.siteMarker = siteMarker;
        VoronoiDiagram.UpdateVoronoi();
    }

    void Update(){
        // update every UPDATE_STEP
        if(pastTime < UPDATE_STEP){
            pastTime  += Time.deltaTime;
        } else {
            VoronoiDiagram.UpdateVoronoi();
            pastTime = 0;
        }
    }
}