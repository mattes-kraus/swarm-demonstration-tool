using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RecordDemonstration : MonoBehaviour, IPointerDownHandler
{
    // constants
    private const int TIMESTEP = 1;
    private const int FINAL_TIMESTEP = 24;
    private const int N_LOCAL_OBS = 10;
    [SerializeField] MetricManagement metricManager;  
    private string path = $"./Metrics_{GameManagement.instance}/";

    // high level
    private bool active = false;
    private int count_timesteps = 0;
    private float timer = 0f;
    private List<Trajectory> trajectories = new();

    // helper
    // private List<int> last_obs = new();
    private List<float> last_orientations = new();


    // Update is called once per frame
    void Update()
    {
        // check if we are currently recording
        if(!active) return;

        // check if we have recorded enough
        if(count_timesteps > FINAL_TIMESTEP){
            StartStopRecording();
            return;
        }

        // manage timeinterval of saving trajectories
        timer += Time.deltaTime;
        if(timer <= TIMESTEP) return;

        // prepare trajectory. we use current actions and last obs, since we want to track
        // with what actions the user reacted to an observation. We measure the actions
        // with 1s delay, therefore those actions belong to the state 1s ago. 
        int i = 0;
        foreach (var robot in GameManagement.allBots)
        {
            ColorTracker curr_bot = robot.GetComponent<ColorTracker>(); 

            // map orientation change from [-359,359] to [-1,1], delta = (curr - last) / 180
            // if delta is above 1 or below -1 we have to clamp the value to the other side
            float deltaOrientation = (robot.transform.eulerAngles.y - last_orientations[i])/180;
            if (deltaOrientation > 1){
                deltaOrientation = deltaOrientation - 2;
            } else if (deltaOrientation < -1) {
                deltaOrientation = deltaOrientation + 2;
            }

            List<float> acts = new()
            {
                curr_bot.realSpeed,
                deltaOrientation
            };
            Debug.Log("delta orientation:" + acts[1]);

            List<float> obs = new();
            // obs.AddRange(robot.GetComponent<LocalMetricTracker>().GetLocalObservations());
            obs.AddRange(Enumerable.Repeat(0f, N_LOCAL_OBS).ToList());
            obs.AddRange(metricManager.GetSwarmMetrics());

            // if its first obs, we need to wait one round with saving since we haven't a last_obs
            if(count_timesteps > 0){
                trajectories[i].acts.Add(acts);
                trajectories[i].obs.Add(obs);
            } else {
                trajectories[i].obs.Add(obs);
            }

            // prepare next timestep
            last_orientations[i] = robot.transform.eulerAngles.y;
            
            // prepare next robot
            i += 1;
        }

        // prepare next trajectory
        timer = 0f;
        count_timesteps += 1;
    }

    public void OnPointerDown(PointerEventData eventData){
        StartStopRecording();
    }

    private void StartStopRecording(){
        active = !active;

        if(active){
            gameObject.GetComponent<Image>().color = Color.green;

            // init trajectory list
            trajectories.Clear();
            foreach (var robot in GameManagement.allBots)
            {
                Trajectory trajectory = new();
                trajectory.acts = new();
                trajectory.obs  = new();
                trajectories.Add(trajectory);
            }

            // init orientation list
            last_orientations.Clear();
            foreach (var robot in GameManagement.allBots)
            {
                last_orientations.Add(robot.transform.rotation.normalized.y);
            }
            
            // init helper variables
            timer = 0;
            count_timesteps = 0;
        } else {
            gameObject.GetComponent<Image>().color = Color.white;
            
            // write demos to hard drive
            foreach (var traj in trajectories)
            {    
                int i = 1;
                string filename = path + "demonstration" + i + ".json";

                while(File.Exists(filename)){
                    i += 1;
                    filename = path + "demonstration" + i + ".json";
                }

                File.WriteAllText(filename, JsonConvert.SerializeObject(traj, Formatting.Indented));
                Debug.Log("wrote demo json");
            }

        }


    }
}
