using System;
using System.Collections;
using UnityEngine;

public class Turtlebot : MonoBehaviour
{
    private float speed = 0.31f; 
    public BotBehavior botState = BotBehavior.Random;
    public Vector3 targetLoc;
    [SerializeField] private GameObject selectedMarker;

    void Start(){
        try{
            GameObject.Find("MetricManager").GetComponent<MetricManagement>().allBots.Add(this);
        } catch(SystemException e){
            Debug.Log(e.Message);
        }
    }

    void Update()
    {
        if(GameManagement.gameState != GameState.Running){
            return;
        }
        
        switch (botState)
        {
            // move constantly around
            case BotBehavior.Random: 
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                break;
            
            // leave from a specified location
            case BotBehavior.Leave: 
                Vector3 leavedirection = transform.position - targetLoc; 
                leavedirection.y = 0;
                transform.LookAt(transform.position + leavedirection);
                transform.Translate(speed * Time.deltaTime * Vector3.forward);
                break;

            // approach target location slowly 
            case BotBehavior.Come:
                // ignoring height of the objects
                Vector2 currPos = new Vector2(transform.position.x, transform.position.z);
                Vector2 targetPos = new Vector2(targetLoc.x, targetLoc.z);
                float dist = Vector2.Distance(currPos, targetPos);

                // approach
                if(dist <= 2*speed){
                    speed =  dist/2;   
                }
                // only drive if not yet at target location +-20cm
                if(dist > 0.2){
                    Vector3 comedirection = targetLoc - transform.position; 
                    comedirection.y = 0;
                    transform.LookAt(transform.position + comedirection);

                    transform.Translate(speed * Time.deltaTime * Vector3.forward);
                }
                break;

            default:
                break;
        }
    }

    // bounce from walls+turtlebots, ignore groundsticker etc.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Flat")){
            return;
        }

        switch(botState){
            case BotBehavior.Random:
                float randomAngle = UnityEngine.Random.Range(0f, 360f);
                transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
                break;
            case BotBehavior.Leave:
                ChangeState(BotBehavior.Random, targetLoc);
                break;
            default:
                break;
        }  
    }

    void OnCollisionStay(Collision collision){
        OnCollisionEnter(collision);
    }

    // selection control: change from selected to unselect or other way round
    public void OnMouseDown(){
        if(!GameManagement.selectedBots.Contains(this)){
            GameManagement.selectedBots.Add(this);
            selectedMarker.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green);
            selectedMarker.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        } else {
            GameManagement.selectedBots.Remove(this);
            selectedMarker.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            selectedMarker.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

        }
        GetComponent<Renderer>().UpdateGIMaterials();
    }

    // called by UI for every selected bot to change some behaviors
    public void ChangeState(BotBehavior state, Vector3 pos){
        botState = state;
        targetLoc = pos;
        
        switch(botState){
            // spread out
            case BotBehavior.Random:
                float randomAngle = UnityEngine.Random.Range(0f, 360f);
                transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
                speed = 0.31f;
                break;

            // move towards specified location
            case BotBehavior.Come:
                speed = 0.31f;
                break;

            // leave from a specified location
            case BotBehavior.Leave:
                speed = 0.31f;
                break;

            default:
                break;
        } 
    }
}

public enum BotBehavior {
    Stop,
    Come,
    Random,
    Leave
}