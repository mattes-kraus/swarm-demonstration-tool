using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.PreprocessedRobot;
using RosMessageTypes.Geometry;
using System.Collections.Generic;
using System.Linq;
using RosMessageTypes.Std;
using System.Text.RegularExpressions;
using System;


public class ROSCommunicator : MonoBehaviour
{

    static ROSConnection ros;
    static private List<Turtlebot> botIterator;
    [SerializeField] private MetricManagement metricManager;
    private static float passedTime;
    private static float episodeLength = 48;
    [SerializeField] private GameObject unnecessaryUI; 
    [SerializeField] private GameObject policyVisuUI; 
    [SerializeField] private GameObject trainingVisuUI; 
    [SerializeField] private Importer importer;
    [SerializeField] private StartDemo demoStartHandler;
    private string arenaName;
    
    private float cycleTime = 0;    // tracks how long we waited since the last action
    private GameState purpose;

    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        // Check if program was started from python
        if (args.Length > 1)
        {
            /* ------------- prepare arguments --------------------------------------- */
            // Regex to extract arguments
            string pattern = @"-(\w+)\s+([^\s]+)";
            var matches = Regex.Matches(args[1], pattern); //args[0] ist Programmname

            // Dictionary to save argument values according to their name
            Dictionary<string, string> parsedArguments = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                string variable = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                parsedArguments[variable] = value;
            }

            /* ------------- evaluate arguments  ------------------------------------- */
            try{
                GameManagement.instance = parsedArguments["instanceName"];
                
                if (parsedArguments["reason"] == "training") {
                    // jump directly to Demonstration and read actions
                    GameManagement.gameState = GameState.Training;

                    // load arena
                    arenaName = parsedArguments["arenaName"];
                    importer.LoadArena(arenaName);

                    // deactivate UI
                    unnecessaryUI.SetActive(false);
                    trainingVisuUI.SetActive(true);

                    // save purpose the user provided so we can correctly reset later
                    purpose = GameState.Training;
                }

                if (parsedArguments["reason"] == "policy_visu") {
                    // jump directly to Demonstration and read actions
                    GameManagement.gameState = GameState.VisualizePolicy;

                    // load arena
                    arenaName = parsedArguments["arenaName"];
                    importer.LoadArena(arenaName);

                    // deactivate UI for arena customization and activate policy visu UI
                    unnecessaryUI.SetActive(false);
                    policyVisuUI.SetActive(true);
                    
                    // save purpose so we can correctly reset later
                    purpose = GameState.VisualizePolicy;
                }

                GameManagement.trail_visu = int.Parse(parsedArguments["trail_visu"]);

            } catch(SystemException e){
                Debug.Log(e);
            }            
        }
    }

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ObservationMsg>("observation");
        ros.Subscribe<TwistMsg>("cmd_vel_unstamped", ReceiveMessage);
        ros.Subscribe<BoolMsg>("reset", ResetSim);
    }

    void Update()
    {
        // track time for check if demo is over. PassedTime will be something about 48 seconds
        if (GameManagement.gameState == GameState.Training){
            passedTime += Time.deltaTime * GameManagement.actionsPerSecond;
        } else {
            passedTime += Time.deltaTime;
        }
    }

    private void ResetSim(BoolMsg message)
    {
        FileHandler.ResetArena();
        GameManagement.gameState = purpose;
        importer.LoadArena(arenaName);
        passedTime = 0;
    }

    private void ReceiveMessage(TwistMsg message)
    {
        Debug.Log("Received message from python: " + message.ToString());

        // --------------- read the new actions -------------------------------
        ActionData actionData = new(
            (float)message.linear.x,
            (float)message.angular.z
        );

        // ------- look which robots turn it is to move -------------------
        if (botIterator?.Any() != true)
        {
            botIterator = new List<Turtlebot>(GameManagement.allBots);
        }
        Turtlebot curr = botIterator.First();

        // --------------- execute the action for one robot ---------------
        // cnn will give rotation [-1,1] where 0 = 0°, -1 = -180 and 1 = 180
        float rotation = actionData.orientation * 180;
        curr.transform.Rotate(0f, rotation, 0f);

        curr.speed = actionData.speed;
        botIterator.Remove(curr);


        // ------- get the observations of the NEXT robot and send 'em ----
        // next robot and not current since the next action will be based
        // on the observations we return here 
        if (botIterator?.Any() != true)
        {
            botIterator = new List<Turtlebot>(GameManagement.allBots);
        }
        curr = botIterator.First();

        // add local and swarm metrics
        ObservationData obs_data = new() { observations = new() };
        obs_data.observations.AddRange(curr.gameObject.GetComponent<LocalMetricTracker>().GetLocalObservations());
        obs_data.observations.AddRange(metricManager.GetSwarmMetrics());

        obs_data.terminated = passedTime > episodeLength;

        // send obs to python
        ros.Publish("observation", new ObservationMsg(
            n_obs: obs_data.observations.Count,
            obs: obs_data.observations.ToArray(),
            terminated: obs_data.terminated
        ));
    }
}