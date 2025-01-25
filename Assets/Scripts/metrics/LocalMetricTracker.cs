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
    private bool collided = false;

    // LIDAR visu
    [SerializeField] LineRenderer lineRenderer;
    public bool visuOn = true;



    void Update(){
        // Visu for robot 1
        if(gameObject.GetComponent<Turtlebot>().indexInAllBots == 1 && visuOn){
            GetCurrentLidarObservations();

            // Berechne die neue Länge des Arrays
            int newSize = closestBots.Length*2 + 1;
            Vector3[] newArray = new Vector3[newSize]; // Neues Array mit mehr Platz

            int arrayIndex = 0;
            for (int i = 0; i < newArray.Length; i++)
            {
                // An geraden Indizes das neue Element einfügen
                if (i % 2 == 0)
                {
                    newArray[i] = transform.position;
                }
                else if (arrayIndex < closestBots.Length)
                {
                    // Restliche Elemente aus dem alten Array kopieren
                    newArray[i] = closestBots[arrayIndex];
                    arrayIndex++;
                }
            }
            lineRenderer.positionCount = DIRECTIONS_TO_OBSERVE*2+1;
            lineRenderer.SetPositions(newArray);
        }
    }

    // ---- All local metrics in one flat numerical vector --------------------
    public List<float> GetLocalObservations(){
        List<float> result = new();

        // speed 
        result.Add(GetCurrentSpeed());

        // color
        result.AddRange(GetCurrentColor());

        // bumper observations
        if(GetCurrentlyBumping()) {result.Add(1f);} else {result.Add(0f);};

        // lidar observations
        foreach (var distance in GetCurrentLidarObservations())
        {
            result.Add(distance*100);
        }

        return result;
    }


    // ------------------ IMU Sensor ------------------------------------------
    private float GetCurrentSpeed(){
        return gameObject.GetComponent<ColorTracker>().realSpeed;
    }


    // ---------------- optical floor tracking sensor -------------------------
    private List<float> GetCurrentColor(){
        ColorWithThreshhold color = gameObject.GetComponent<ColorTracker>().currentColor;

        switch(color)
        {
            case ColorWithThreshhold.Nothing:
                return new List<float> { 1, 0, 0 };
            case ColorWithThreshhold.White:
                return new List<float> { 0, 1, 0 };
            case ColorWithThreshhold.Black:
                return new List<float> { 0, 0, 1 };
            default:
                return new List<float> { 0, 0, 0};
        }

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

            float angleInRadians = (Mathf.Deg2Rad * (i * 360/DIRECTIONS_TO_OBSERVE) + 180)% 360; // Grad in Bogenmaß
            float x = transform.position.x + LIDAR_RADIUS * Mathf.Cos(angleInRadians);
            float z = transform.position.z + LIDAR_RADIUS * Mathf.Sin(angleInRadians);
            closestBots[i] = new Vector3(x, transform.position.y, z);
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

        return lidar;
    }


    // ---------------- bumper sensoric ---------------------------------------
    private bool GetCurrentlyBumping(){
        if(collided){
            collided = false;
            return true;
        } else{ 
            return false;
        }
    }
    void OnCollisionEnter(Collision collisionInfo){
        if (collisionInfo.gameObject.CompareTag("Flat")){
            return;
        }

        collided = true;
    }
}
