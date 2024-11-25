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
        if(GameManagement.gameState == GameState.Training){

            string acts_path = "./Metrics_test_instance/agent_actions.json";
            string obs_path = "./Metrics_test_instance/agent_observations.json";

            // check if new actions came in
            string rawObservations = File.ReadAllText(obs_path);
            ObservationData observations = JsonUtility.FromJson<ObservationData>(rawObservations);
            if (observations == null || observations.read) return;


            // read the new actions
            // string actions = File.ReadAllText("./Build/Metrics_" + GameManagement.instance + "/agent_actions.json");
            string actions = File.ReadAllText(acts_path);
            Root actionData = JsonUtility.FromJson<Root>(actions);

            // execute the actions
            debugTextfield.text = actionData.action_data.Count.ToString();
            for(int i = 0; i < GameManagement.allBots.Count; i++)
            {
                // check that there are not more robots than specified actions
                if(i < actionData.action_data.Count){
                    ActionData current_actions = actionData.action_data[i];

                    // rotate and move the robots according to the read json file
                    GameManagement.allBots[i].transform.Rotate(0f, current_actions.orientation, 0f);
                    GameManagement.allBots[i].transform.Translate(Vector3.forward * current_actions.speed); 
                    transform.position = new Vector3(Clamp(transform.position.x), transform.position.y, Clamp(transform.position.z));   
                }
            }

            // if(GameManagement.gameState == GameState.Training)
            //     GameManagement.allBots.ForEach((robot) => {
            //         robot.botState = BotBehavior.Random;
            // });
            
            Debug.Log("Iteration: " + actionData.iteration);
            foreach (var agent in actionData.action_data){
                Debug.Log(agent.orientation + ", " + agent.speed);
            }
            Debug.Log(GameManagement.allBots.Count);
            foreach (var robot in GameManagement.allBots){
                Debug.Log(robot);
            }

            // commmunicate with python that we read the new actions
            ObservationData newData = new() { read = true};
            File.WriteAllText(obs_path, JsonUtility.ToJson(newData));
        }
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