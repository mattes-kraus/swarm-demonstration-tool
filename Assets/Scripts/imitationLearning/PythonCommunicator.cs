using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;


// Communicates with python to get actions and send observations
public class PythonCommunicator : MonoBehaviour
{   

    [SerializeField] private TMP_Text debugTextfield; 
    [SerializeField] private GameObject unnecessaryUI; 
    [SerializeField] private GameObject policyVisuUI; 
    [SerializeField] private GameObject trainingVisuUI; 
    [SerializeField] private Importer importer;
    [SerializeField] private MetricManagement metricManager;
    [SerializeField] private StartDemo demoStartHandler;
    
    private float passedTime = 0;   // tracks how long the demo already is
    private float cycleTime = 0;    // tracks how long we waited since the last action
    private GameState purpose;

    private string arenaName;


    // Evaluate the arguments the simulation is started with
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
                    policyVisuUI.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Policy visu in " + arenaName;
                    
                    // save purpose so we can correctly reset later
                    purpose = GameState.VisualizePolicy;
                }

                GameManagement.trail_visu = int.Parse(parsedArguments["trail_visu"]);


            } catch(SystemException e){
                Debug.Log(e);
            }            
        }
    }

    void Update(){
        // also, we just want python to take actions each second per robot. With speed up,
        // each second divided by actions_per_second. 
        if( 1f / GameManagement.allBots.Count > cycleTime)
            return;
        else
            cycleTime = 0;

        // if we reached this code block, we actually take an action
        if(GameManagement.gameState == GameState.Training 
        || GameManagement.gameState == GameState.VisualizePolicy){

            // paths
            string acts_path = $"./Metrics_{GameManagement.instance}/agent_actions.json";
            string obs_path  = $"./Metrics_{GameManagement.instance}/agent_observations.json";
            string exec_path = $"./Metrics_{GameManagement.instance}/actions_executed.json";

            // --------------- check if we have to restart --------------------
            bool done = false;
            ExecutedData exec_data = new();
            while(!done){
                try{
                    string rawObservations = File.ReadAllText(exec_path);
                    exec_data = JsonUtility.FromJson<ExecutedData>(rawObservations);
                    done = true;
                } catch (Exception){
                    Debug.Log("wait for reading execution file");
                }
            }

            if (exec_data == null) return;
            
            if (exec_data.reset) {
                // reset if we have to
                FileHandler.ResetArena();
                GameManagement.gameState = purpose;
                importer.LoadArena(arenaName);
                passedTime = 0;

                // communicate our restart
                ExecutedData reset_data = new() { executed = true, reset = false};
                File.WriteAllText(exec_path, JsonUtility.ToJson(reset_data, false));
                return;
            }
        }
    }
}