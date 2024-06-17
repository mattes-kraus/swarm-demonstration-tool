using System.Collections.Generic;
using UnityEngine;

using csDelaunay;
 
public class VoronoiDiagram {
 
    // The number of polygons/sites we want
    public int polygonNumber = 200;
 
    // This is where we will store the resulting data
    private Dictionary<System.Numerics.Vector2, Site> sites;
    private List<csDelaunay.Edge> edges;
 
    void Start() {
        // Create your sites (lets call that the center of your polygons)
        List<System.Numerics.Vector2> points = new();
        points.Add(new System.Numerics.Vector2(15,10));
        points.Add(new System.Numerics.Vector2(20,10));
        
        //CreateRandomPoint();
       
        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0,0,40,40);
       
        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points,bounds,5);
 
        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);
 
        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;

        foreach (KeyValuePair<System.Numerics.Vector2, Site> item in sites)
        {
            Debug.Log(item.Key+"=>" + item.Value.ToString());               
        } 
        foreach (var item in edges)
        {
            Debug.Log(item);
        }
    }
   
    private List<System.Numerics.Vector2> CreateRandomPoint() {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<System.Numerics.Vector2> points = new List<System.Numerics.Vector2>();
        for (int i = 0; i < polygonNumber; i++) {
            points.Add(new System.Numerics.Vector2(Random.Range(0,512), Random.Range(0,512)));
        }
 
        return points;
    }
 
}    