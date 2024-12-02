using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LocalMetricTracker : MonoBehaviour
{
    // constants
    private const int DIRECTIONS_TO_OBSERVE = 5;
    private const float LIDAR_RADIUS = 1.0f;
    
    // helper variables
    private float[] lidar = new float[DIRECTIONS_TO_OBSERVE];
    private Vector3[] closestBots = new Vector3[DIRECTIONS_TO_OBSERVE];
    private bool colliding = false;

    // LIDAR visu
    [SerializeField] LineRenderer lineRenderer;

    // ---- All local metrics in one flat numerical vector --------------------
    public List<int> GetLocalObservations(){
        List<int> result = new();

        // speed 
        result.Add((int)(GetCurrentSpeed()*100));

        // color
        result.Add((int)GetCurrentColor());

        // bumper observations
        if(GetCurrentlyBumping()) {result.Add(1);} else {result.Add(0);};

        // lidar observations
        foreach (var distance in GetCurrentLidarObservations())
        {
            result.Add((int)(distance*100));
        }

        return result;
    }


    // ------------------ IMU Sensor ------------------------------------------
    private float GetCurrentSpeed(){
        return gameObject.GetComponent<ColorTracker>().realSpeed;
    }


    // ---------------- optical floor tracking sensor -------------------------
    private ColorWithThreshhold GetCurrentColor(){
        return gameObject.GetComponent<ColorTracker>().lastColor;
    }


    // ------------------ LIDAR -----------------------------------------------
    // - abstracted to k orientations where the closest robot distance is 
    // - measured  
    private float[] GetCurrentLidarObservations(){
        // init calculations
        int index;
        float angle;
        Vector3 thisBotPos = transform.position;
        Vector3 helperPos;
        thisBotPos.y = 0;

        // reset distances
        for(int i = 0; i < DIRECTIONS_TO_OBSERVE; i++){
            lidar[i] = LIDAR_RADIUS;
            closestBots[i] = transform.position;
        }

        // do the LIDAR abstraction
        foreach (var bot in GameManagement.allBots){
            // don't do LIDAR on itself
            if(bot.transform == transform) continue;

            // don't care about height of bot when measuring angle
            helperPos = bot.transform.position;
            helperPos.y = 0;
            
            // calculate angle between this bot and the other
            helperPos = helperPos - thisBotPos;
            if(Mathf.Abs(helperPos.magnitude) > LIDAR_RADIUS) continue;
            angle = Mathf.Atan2(helperPos.z, helperPos.x) * Mathf.Rad2Deg + 180;

            // see in what orientation the other robot lies
            index = (int)(angle * DIRECTIONS_TO_OBSERVE / 360);

            // update shortest distance in that direction
            if(Mathf.Abs(helperPos.magnitude) < lidar[index]){
                lidar[index] = Mathf.Abs(helperPos.magnitude);
                closestBots[index] = transform.position + helperPos;
            }
        }

        // Visu
        lineRenderer.positionCount = DIRECTIONS_TO_OBSERVE;
        lineRenderer.SetPositions(closestBots);

        return lidar;
    }


    // ---------------- bumper sensoric ---------------------------------------
    private bool GetCurrentlyBumping(){
        return colliding;
    }
    void OnCollisionStay(Collision collisionInfo){
        if (collisionInfo.gameObject.CompareTag("Flat")){
            return;
        }

        colliding = true;
    }
    void OnCollisionExit(Collision collisionInfo){
        if (collisionInfo.gameObject.CompareTag("Flat")){
            return;
        }

        colliding = false;
    }
}
