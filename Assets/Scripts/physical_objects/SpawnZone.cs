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
                
                float newX = Clamp(transform.position.x + noise_x, GameManagement.ARENA_X_MIN, GameManagement.ARENA_X_MAX);
                float newZ = Clamp(transform.position.z + noise_z, GameManagement.ARENA_Z_MIN, GameManagement.ARENA_Z_MAX);

                robot.transform.SetPositionAndRotation(new Vector3(newX, robotPos.y, newZ), Quaternion.identity);   

                // termination condition
                retries++;
                timeCounter = 0;
            } else {
                // stand still for three seconds to be sure every robot spawned correctly
                timeCounter += Time.deltaTime;
                if(timeCounter >= 3) {
                    spawned = true;
                    renderer1.enabled = false;
                    renderer2.enabled = false;
                    gameObject.layer = 2;
                }
            }
        } 

        if(GameManagement.gameState == GameState.Running){
            spawned = true;
            renderer1.enabled = false;
            renderer2.enabled = false;
            gameObject.layer = 2;
        }
    }

    // destroy robots which are not in the spwan zone anymore
    void OnTriggerExit(Collider collider){
        if(collider.gameObject.CompareTag("Bot") && !spawned){
            Destroy(collider.gameObject);
            currentRobots--;
        }
    }

    private float Clamp(float value, float min, float max)
    {
        return Math.Max(min, Math.Min(value, max));
    }
}
