using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class Turtlebot : MonoBehaviour
{
    public float speed = 0; 
    public BotBehavior botState = BotBehavior.Random;
    public Vector3 targetLoc;
    public int indexInAllBots; // to get position in voronoi diagram
    [SerializeField] private GameObject selectedMarker;
    [SerializeField] private GameObject stateIndicator;
    private const float MAX_SPEED = 0.31f; 
    private const float borderPuffer = 0.2f; 

    void Start(){
        indexInAllBots = GameManagement.AddBotToGlobalList(this);
        Debug.Log(indexInAllBots);
    }

    void Update()
    {
        // speed up robot so he moves on max speed per frame instead of per second
        if(GameManagement.gameState == GameState.Training){
            transform.Translate(Vector3.forward * speed * Time.deltaTime * GameManagement.actionsPerSecond);
            transform.position = new Vector3(
                Clamp(transform.position.x),
                transform.position.y, 
                Clamp(transform.position.z)); 
            return;
        }

        // move robots in realtime so user can see what policy is doing 
        // if(GameManagement.gameState == GameState.VisualizePolicy){
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            transform.position = new Vector3(
                Clamp(transform.position.x),
                transform.position.y, 
                Clamp(transform.position.z));
            return;
        // }

        // stop moving and updating when game is not running
        if(GameManagement.gameState != GameState.Running){
            return;
        }

        // unselect in case we are in beacon control right now
        if(GameManagement.currentControlMode != ControlMode.Selection){
            GameManagement.selectedBots.Remove(this);
            selectedMarker.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            selectedMarker.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            selectedMarker.GetComponent<Renderer>().UpdateGIMaterials();
        }

        // indicate robot state via light color
        stateIndicator.GetComponent<Renderer>().material.SetColor("_EmissionColor", GameManagement.GetColorByBehavior(botState));
        stateIndicator.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        stateIndicator.GetComponent<Renderer>().UpdateGIMaterials();
        
        // execute behavior depending on its state
        switch (botState)
        {
            // move constantly around
            case BotBehavior.Random: 
                speed = MAX_SPEED;
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                break;
            
            // leave from a specified location
            case BotBehavior.Leave: 
                speed = MAX_SPEED;
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

                // approach location slowly
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

            case BotBehavior.Stop:
                speed = 0;
                break;

            case BotBehavior.Deploy:
                speed = MAX_SPEED;
                
                Vector3 newPos = VoronoiDiagram.GetDeployPos(indexInAllBots);
                newPos.y = transform.position.y;
                transform.LookAt(newPos);
                transform.Translate(speed * Time.deltaTime * Vector3.forward);
                break;

            default:
                speed = MAX_SPEED;
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
                // switch direction randomly
                float randomAngle = UnityEngine.Random.Range(0f, 360f);
                transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
                break;
            case BotBehavior.Leave:
                // go to random, hope we are far enough away from location
                ChangeState(BotBehavior.Random, targetLoc);
                break;
            case BotBehavior.Deploy:
                // switch index with collision partner
                // if(collision.gameObject.tag == "Bot"){
                //     Turtlebot otherBot = collision.gameObject.GetComponent<Turtlebot>();
                //     int helper = otherBot.indexInAllBots;
                //     otherBot.indexInAllBots = indexInAllBots;
                //     indexInAllBots = helper;
                // }
                break;
            default:
                break;
        }  
    }

    void OnCollisionStay(Collision collision){
        switch(botState){
            case BotBehavior.Random:
                OnCollisionEnter(collision);
                break;
            case BotBehavior.Leave:
                OnCollisionEnter(collision);
                break;
            case BotBehavior.Deploy:
                if(collision.gameObject.CompareTag("Bot")){
                    transform.Translate(Vector3.right*Time.deltaTime * speed);
                }
                break;
            default: 
                break;
        }
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
                speed = MAX_SPEED;
                break;

            // move towards specified location
            case BotBehavior.Come:
                speed = MAX_SPEED;
                break;

            // leave from a specified location
            case BotBehavior.Leave:
                speed = MAX_SPEED;
                break;

            default:
                break;
        } 
    }

    void OnDestroy(){
        GameManagement.allBots.Remove(this);
    }

    float Clamp(float x){
        if(x >= GameManagement.ARENA_X_MAX - borderPuffer){
            return GameManagement.ARENA_X_MAX - borderPuffer;
        } else if(x < GameManagement.ARENA_X_MIN + borderPuffer){
            return GameManagement.ARENA_X_MIN + borderPuffer;
        } else {
            return x;
        }
    }
}

public enum BotBehavior {
    Stop,
    Come,
    Random,
    Leave,
    Deploy
}

