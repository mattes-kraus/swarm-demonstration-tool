using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;


public class PythonCommunicator : MonoBehaviour
{   

    [SerializeField] private TMP_Text debugTextfield; 
    [SerializeField] private Importer importer;
    [SerializeField] private MetricManagement metricManager;
    [SerializeField] private StartDemo demoStartHandler;
    private List<Turtlebot> botIterator;
    private float passedTime = 0;   // tracks how long the demo already is
    private float cycleTime = 0;    // tracks how long we waited since the last action
    private GameState purpose;

    private string arenaName;

    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        // Check ob Programm aus Python gestartet wurde
        if (args.Length > 1)
        {
            /* ------------- Argumente vorbereiten ----------------------------------- */
            // Regex zum Extrahieren von Variablen und Werten
            string pattern = @"-(\w+)\s+([^\s]+)";
            var matches = Regex.Matches(args[1], pattern); //args[0] ist Programmname

            // Dictionary zum Speichern der Variablen-Werte-Paare
            Dictionary<string, string> parsedArguments = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                string variable = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                parsedArguments[variable] = value;
            }

            /* ------------- Argumente auswerten ------------------------------------- */
            try{
                GameManagement.instance = parsedArguments["instanceName"];
                
                if (parsedArguments["reason"] == "training") {
                    // springe direkt zur Demonstration und lese in intervallen aktionen aus
                    GameManagement.gameState = GameState.Training;

                    // arena laden, hässlich da Importer auch gleichzeitig import button ist
                    arenaName = parsedArguments["arenaName"];
                    importer.LoadArena(arenaName);

                    // save purpose so we can correctly reset later
                    purpose = GameState.Training;
                }

                if (parsedArguments["reason"] == "policy_visu") {
                    // springe direkt zur Demonstration und lese in intervallen aktionen aus
                    GameManagement.gameState = GameState.VisualizePolicy;

                    // arena laden, hässlich da Importer auch gleichzeitig import button ist
                    arenaName = parsedArguments["arenaName"];
                    importer.LoadArena(arenaName);
                    
                    // save purpose so we can correctly reset later
                    purpose = GameState.VisualizePolicy;
                }


            } catch(SystemException e){
                Debug.Log(e);
            }            
        }
    }

    void Update(){
        // track time for check if demo is over. PassedTime will be something about 48 seconds
        if (GameManagement.gameState == GameState.Training){
            passedTime += Time.deltaTime * GameManagement.actionsPerSecond;
            cycleTime  += Time.deltaTime * GameManagement.actionsPerSecond;
        } else {
            passedTime += Time.deltaTime;
            cycleTime  += Time.deltaTime;
        }

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

            // --------------- if no new actions came in return  --------------
            if (exec_data.executed) return;

            // --------------- else read the new actions ----------------------
            string actions = File.ReadAllText(acts_path);
            ActionData actionData = JsonUtility.FromJson<ActionData>(actions);
            
            // ------- look which robots turn it is to move -------------------
            if(botIterator?.Any() != true){
                botIterator = new List<Turtlebot>(GameManagement.allBots);
            }
            Turtlebot curr = botIterator.First();
            
            // --------------- execute the action for one robot ---------------
            // cnn will give rotation [-1,1] where 0 = 0°, -1 = -180 and 1 = 180
            float rotation = actionData.orientation * 180 ; 
            curr.transform.Rotate(0f, rotation, 0f);

            curr.speed = actionData.speed;  
            botIterator.Remove(curr);

            // debug
            // debugTextfield.text = rotation + " " + (float) rotation + " " + actionData.speed + " " + (float)actionData.speed;

            // ------- get the observations of the NEXT robot and send 'em ----
            // next robot and not current since the next action will be based
            // on the observations we return here 
            if(botIterator?.Any() != true){
                botIterator = new List<Turtlebot>(GameManagement.allBots);
            }
            curr = botIterator.First();

            // add local and swarm metrics
            ObservationData obs_data = new() { observations =  new()};
            obs_data.observations.AddRange(curr.gameObject.GetComponent<LocalMetricTracker>().GetLocalObservations());
            obs_data.observations.AddRange(metricManager.GetSwarmMetrics());
            
            // check if the demonstration is over
            // if(GameManagement.gameState == GameState.Training){
            //     obs_data.terminated = passedTime > (48f/(float)GameManagement.actionsPerSecond);
            // } else {
                obs_data.terminated = passedTime > 48;
            // }

            // send obs to python
            done = false;
            while(!done){
                try{
                    File.WriteAllText(obs_path, JsonUtility.ToJson(obs_data, false));
                    done = true;
                } catch (Exception) {
                    Debug.Log("wait for obs permission");
                }
            }

            // -- commmunicate with python that we executed the new actions ---
            exec_data = new() { executed = true};
            File.WriteAllText(exec_path, JsonUtility.ToJson(exec_data, false));
        }
    }
}