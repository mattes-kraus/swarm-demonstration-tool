using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private GameObject turtlebot;
    [SerializeField] private Renderer renderer1;
    [SerializeField] private Renderer renderer2;
    private Transform parent;

    public int robotsToSpawn = 3;
    private int currentRobots = 0;
    private bool spawned = false;
    private int retries = 0;
    private float timeCounter = 0;

    void Start(){
        parent = GameObject.Find("Robots").transform;
    }


    void Update()
    {
        // spawn robots when demo is started
        if(!spawned && GameManagement.gameState == GameState.Paused){

            if(currentRobots < robotsToSpawn && retries <= 10*robotsToSpawn)
            {
                // spawn robot at 0/0
                GameObject robot = Instantiate(turtlebot, parent);
                Vector3 robotPos = robot.transform.position;
                currentRobots++;

                // move robot to its spawn zone
                float noise_x = UnityEngine.Random.Range(-transform.localScale.x/2, transform.localScale.x/2);
                float noise_z = UnityEngine.Random.Range(-transform.localScale.z/2, transform.localScale.z/2);
                robot.transform.SetPositionAndRotation(new Vector3(transform.position.x + noise_x, robotPos.y, transform.position.z + noise_z), Quaternion.identity);   

                // abbruchbedingungen
                retries++;
                timeCounter = 0;
            } else {
                // 3 sekunden still in der spawn zone stehen zum abbruch
                timeCounter += Time.deltaTime;
                if(timeCounter >= 3) {
                    spawned = true;
                }
            }
        } 
        // make invisible when game is running
        renderer1.enabled = GameManagement.gameState != GameState.Running;
        renderer2.enabled = GameManagement.gameState != GameState.Running;
    }

    void OnTriggerExit(Collider collider){
        if(collider.gameObject.CompareTag("Bot") && !spawned){
            Destroy(collider.gameObject);
            currentRobots--;
        }
    }
}
