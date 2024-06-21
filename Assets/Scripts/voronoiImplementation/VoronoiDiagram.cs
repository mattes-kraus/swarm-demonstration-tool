using System.Collections.Generic;
using UnityEngine;

using csDelaunay;
using System;
using UnityEditor;

public static class VoronoiDiagram {
    // visu
    private static List<GameObject> visuElements = new();
    public static bool visuOn;
    public static GameObject siteMarker;
    
 
    // This is where we will store the resulting data
    private static Dictionary<System.Numerics.Vector2, Site> sites;
    private static List<Edge> edges;
    private static List<System.Numerics.Vector2> points = new();

    public static List<GameObject> walls = new();
 
    public static void UpdateVoronoi(){
            // --- Create your sites (robots and walls) -----------------------
            // clear and add robots
            points.Clear();
            GameManagement.allBots.ForEach((bot) => {
                points.Add(new System.Numerics.Vector2(bot.transform.position.x, bot.transform.position.z));
            });

            // add walls
            // Renderer renderer;
            // walls.ForEach((wall) => {
            //     try{
            //         renderer = wall.GetComponent<Renderer>();
            //         points.Add(new System.Numerics.Vector2(renderer.bounds.min.x, renderer.bounds.min.z));
            //         points.Add(new System.Numerics.Vector2(renderer.bounds.max.x, renderer.bounds.max.z));
            //         points.Add(new System.Numerics.Vector2(wall.transform.position.x, wall.transform.position.z));
            //     } catch (SystemException){
            //         Debug.Log("wall has been deleted");
            //     }
            // });
        
            // Create the bounds of the voronoi diagram
            Rectf bounds = new Rectf(
                GameManagement.ARENA_X_MIN,
                GameManagement.ARENA_Z_MIN,
                GameManagement.ARENA_X_MAX - GameManagement.ARENA_X_MIN,
                GameManagement.ARENA_Z_MAX - GameManagement.ARENA_Z_MIN
            );
        
            // calculate voronoi diagram 
            Voronoi voronoi = new Voronoi(points,bounds,5);

            // sites = centroids, edges = edges
            sites = voronoi.SitesIndexedByLocation;
            edges = voronoi.Edges;

            // visu
            visuElements.ForEach((marker) => {
                    GameObject.Destroy(marker);
                });
                
            if(visuOn){
                foreach (KeyValuePair<System.Numerics.Vector2, Site> kvp in sites)
                {
                    if(float.IsNormal(kvp.Value.x) && float.IsNormal(kvp.Value.y))
                    visuElements.Add(GameObject.Instantiate(siteMarker,  new Vector3(kvp.Value.x, 2, kvp.Value.y), Quaternion.identity));
                }
            }
    }

    public static Vector3 GetDeployPos(int index) {
        Debug.Log(index);

        // search for that index in all sites
        foreach (var kvp in sites)
        {
            if(kvp.Value.SiteIndex == index) return new Vector3(kvp.Value.x, 0, kvp.Value.y);
        }

        return Vector3.zero;
    }

 
    // static void OnDrawGizmosSelected()
    // {
    //     Debug.Log("Gizmo called");

    //     // Create your sites (lets call that the center of your polygons)
    //      //CreateRandomPoint(); //
    //     points.Clear();
    //     points.Add(new System.Numerics.Vector2(-1,0));
    //     points.Add(new System.Numerics.Vector2(-2,1));
    //     points.Add(new System.Numerics.Vector2(-2,0));
    //     points.Add(new System.Numerics.Vector2(-2,-1));
    //     points.Add(new System.Numerics.Vector2(2,1));
    //     points.Add(new System.Numerics.Vector2(2,0));
    //     points.Add(new System.Numerics.Vector2(2,-1));
       
    //     // Create the bounds of the voronoi diagram
    //     // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
    //     // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
    //     Rectf bounds = new Rectf(-2,-2,4,4);
       
    //     // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
    //     // Here I used it with 2 iterations of the lloyd relaxation
    //     Voronoi voronoi = new Voronoi(points,bounds,5);
 
    //     // But you could also create it without lloyd relaxtion and call that function later if you want
    //     //Voronoi voronoi = new Voronoi(points,bounds);
    //     //voronoi.LloydRelaxation(5);
 
    //     // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
    //     sites = voronoi.SitesIndexedByLocation;
    //     edges = voronoi.Edges;

    //     if (edges != null)
    //     {
    //         // Draw Voronoi edges
    //         Gizmos.color = Color.green;
    //         foreach (Edge edge in edges)
    //         {
    //             System.Numerics.Vector2? start = edge.ClippedEnds[LR.LEFT];
    //             System.Numerics.Vector2? end = edge.ClippedEnds[LR.RIGHT];

    //             if (start.HasValue && end.HasValue)
    //             {
    //                 Gizmos.DrawLine(new Vector3(start.Value.X, 2, start.Value.Y), new Vector3(end.Value.X, 2, end.Value.Y));
    //             }
    //         }

    //         // Draw robot positions
    //         Gizmos.color = Color.blue;
    //         foreach (System.Numerics.Vector2 robotPosition in points)
    //         {
    //             Gizmos.DrawSphere(new Vector3(robotPosition.X, 2, robotPosition.Y), 0.2f);
    //         }

    //         // draw centroids
    //         Gizmos.color = Color.green;
    //         foreach (KeyValuePair<System.Numerics.Vector2, Site> kvp in sites)
    //         {
    //             Gizmos.DrawSphere(new Vector3(kvp.Value.x, 2, kvp.Value.y), 0.2f);
    //         }
    //     }
    // }
}  