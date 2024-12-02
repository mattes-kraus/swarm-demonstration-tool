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
    private List<Turtlebot> botIterator;

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
                    importer.LoadArena(parsedArguments["arenaName"]);
                }
            } catch(SystemException e){
                Debug.Log(e);
            }            
        }
    }

    void Update(){
        //if(GameManagement.gameState == GameState.Training){
            // paths
            string acts_path = $"./Metrics_{GameManagement.instance}/agent_actions.json";
            string obs_path  = $"./Metrics_{GameManagement.instance}/agent_observations.json";
            string exec_path = $"./Metrics_{GameManagement.instance}/actions_executed.json";

            // --------------- check if new actions came in -------------------
            string rawObservations = File.ReadAllText(exec_path);
            ExecutedData observations = JsonUtility.FromJson<ExecutedData>(rawObservations);
            if (observations == null || observations.executed) return;

            // -------------------- read the new actions ----------------------
            string actions = File.ReadAllText(acts_path);
            ActionData actionData = JsonUtility.FromJson<ActionData>(actions);

            // ------- look which robots turn it is to move -------------------
            if(botIterator?.Any() != true){
                botIterator = new List<Turtlebot>(GameManagement.allBots);
            }
            Turtlebot curr = botIterator.First();

            // --------------- execute the action for one robot ---------------
            curr.transform.Rotate(0f, actionData.orientation, 0f);
            curr.transform.Translate(Vector3.forward * actionData.speed); 
            curr.transform.position = new Vector3(
                Clamp(curr.transform.position.x), 
                curr.transform.position.y, 
                Clamp(curr.transform.position.z));   
            botIterator.Remove(curr);

            // ------- get the observations of the robot and send 'em ---------
            List<int> obs = new();
            try{
                obs = curr.gameObject.GetComponent<LocalMetricTracker>().GetLocalObservations();
                ObservationData obs_data = new() { observations = obs };
                File.WriteAllText(obs_path, JsonUtility.ToJson(obs_data, false));
            }
            catch (System.Exception e){
                Debug.Log(e.Message);
            }

            // -- commmunicate with python that we executed the new actions ---
            ExecutedData exec_data = new() { executed = true};
            File.WriteAllText(exec_path, JsonUtility.ToJson(exec_data, false));
        //}
    }

    float Clamp(float x){
        if(x > GameManagement.ARENA_X_MAX){
            return GameManagement.ARENA_X_MAX;
        } else if(x < GameManagement.ARENA_X_MIN){
            return GameManagement.ARENA_X_MIN;
        } else {
            return x;
        }
    }
}