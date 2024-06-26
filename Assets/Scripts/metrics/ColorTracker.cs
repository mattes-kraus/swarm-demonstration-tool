using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ColorTracker : MonoBehaviour
{

    private MetricManagement metricManager;
    //--- speed metric --------------------------------------------------------
    public float realSpeed = 0;
    Vector3 lastPos;
    float updateRate = 0.5f;
    float updateProgress = 0f;

    //--- color switch time --------------------------------------------------
    bool countTime;
    float currentTime = 0;
    private List<float> colorSwitchTimes = new(0);

    //--- what colors visited metric ------------------------------------------
    private ColorWithThreshhold lastColor = ColorWithThreshhold.Nothing;

    void Start(){
        metricManager = GameObject.Find("MetricManager").GetComponent<MetricManagement>();
    }

    void Update(){
        // count travel time
        if(countTime && GameManagement.gameState == GameState.Running){
            currentTime += Time.deltaTime;
        }

        // measure real speed
        updateProgress += Time.deltaTime;
        if(updateProgress >= updateRate){
            realSpeed = Vector3.Distance(lastPos, transform.position) / updateRate;
            realSpeed = math.abs(realSpeed);

            lastPos = transform.position;
            updateProgress = 0;
        }
    }

    void OnTriggerEnter(Collider collider){
        try{
            // if we hit a groundsticker we need to update our color metrics
            if(collider.gameObject.GetComponent<PhysicalObject>().type == ObjectType.Groundsticker){

                // update last color metric but only if we haven't been already on that sticker
                if(lastColor == ColorWithThreshhold.Nothing || !countTime) return;

                ColorWithThreshhold newColor = ApplyThreshhold(collider.gameObject.GetComponent<Renderer>().material.color);
                
                int index = 0;
                if(lastColor == ColorWithThreshhold.Black) index += 2;
                if(newColor  == ColorWithThreshhold.Black) index += 1;
                
                metricManager.colorVisits[index] += 1;

                // update noColorTime
                colorSwitchTimes.Add(currentTime);
                currentTime = 0;
                metricManager.avgColorSwitchTimes.Add(colorSwitchTimes.Average());
                countTime = false;
            }
        } catch (SystemException){
            // not too bad, then collision is just not with groundsticker
        }
    }

    void OnTriggerExit(Collider collider){
        try{
            if(collider.gameObject.GetComponent<PhysicalObject>().type == ObjectType.Groundsticker){
                // update noColorTime
                countTime = true;
                
                // update last color metric
                lastColor = ApplyThreshhold(collider.gameObject.GetComponent<Renderer>().material.color);
            }
        } catch (SystemException){
            // not too bad, then collision is just not with groundsticker
        }
    }

    private ColorWithThreshhold ApplyThreshhold(Color color){
        if(color.r < 35f/255) {
            return ColorWithThreshhold.Black;
        } else if(color.r > 220f/255){
            return ColorWithThreshhold.White;
        } else {
            return ColorWithThreshhold.Nothing;
        }
    }
}

public enum ColorWithThreshhold{
    Black,
    White,
    Nothing
}