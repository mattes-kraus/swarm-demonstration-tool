using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private GameObject turtlebot;
    [SerializeField] private Renderer renderer1;
    [SerializeField] private Renderer renderer2;
    private Transform parent;

    public int robotsToSpawn = 3;
    private bool spawned = false;
    private int retries = 0;

    private System.Random random = new System.Random();

    void Start(){
        parent = GameObject.Find("Robots").transform;
        // transform.localScale = new UnityEngine.Vector3(1.5f, transform.localScale.y, 1.5f);
    }


    void Update()
    {
        // spawn robots when demo is started
        if(!spawned && (GameManagement.gameState == GameState.Paused || GameManagement.gameState == GameState.Training)){

            PlaceRobots(robotsToSpawn, (transform.position.x, transform.position.z), transform.localScale.x/2);

            renderer1.enabled = false;
            renderer2.enabled = false;
            gameObject.layer = 2;
        }
    }

    void PlaceRobots(int n, (float x, float y) bigCircleCenter, float bigCircleRadius)
    {
        var robots = new List<(float x, float y)>();

        // roboter positionen berechnen
        while (robots.Count < n && retries < 1000)
        {
            // Generiere zufällige Position innerhalb eines größeren Quadrats um den großen Kreis
            float x = (float)random.NextDouble() * (2 * bigCircleRadius) - bigCircleRadius + bigCircleCenter.x;
            float y = (float)random.NextDouble() * (2 * bigCircleRadius) - bigCircleRadius + bigCircleCenter.y;

            var newRobot = (x, y);

            if (IsValidPosition(newRobot, robots, bigCircleCenter, bigCircleRadius))
            {
                robots.Add(newRobot);
            }
            retries += 1;
        }

        // robots tatsächlich spawnen
        robots.ForEach((position) => {
            Instantiate(turtlebot, new UnityEngine.Vector3(position.x, turtlebot.transform.position.y, position.y), UnityEngine.Quaternion.identity, parent);
        });

        spawned = true;
    }

    bool IsValidPosition((float x, float y) newRobot, List<(float x, float y)> robots, 
                                (float x, float y) bigCircleCenter, float bigCircleRadius)
    {
        float cx = bigCircleCenter.x, cy = bigCircleCenter.y;
        float nx = newRobot.x, ny = newRobot.y;

        // Prüfen, ob der Roboter innerhalb des großen Kreises ist
        if (Math.Sqrt((nx - cx) * (nx - cx) + (ny - cy) * (ny - cy)) > bigCircleRadius)
        {
            return false;
        }

        // Prüfen, ob der Roboter mit anderen Robotern kollidiert
        foreach (var robot in robots)
        {
            double rx = robot.x, ry = robot.y;
            if (Math.Sqrt((nx - rx) * (nx - rx) + (ny - ry) * (ny - ry)) < 0.34) // ein roboter hat 17cm radius
            {
                return false;
            }
        }

        return true;
    }
}
