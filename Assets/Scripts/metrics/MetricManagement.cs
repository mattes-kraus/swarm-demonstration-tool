using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MetricManagement: MonoBehaviour
{
    // --- actual metrics -----------------------------------------------------
    public float avgDistToCenter;
    public float avgSpeed;
    public float[,] lastVisitTime = new float[N_GRIDS_X, N_GRIDS_Z];
    public float avgColorSwitchTime;


    // --- color metrics ------------------------------------------------------
    // whitewhite:0, whiteblack:1, blackwhite:2, blackblack:3
    public float[] colorVisits = new float[4];
    
    // -- metric helpers ------------------------------------------------------
    private float totalDistToCenter;
    public List<float> avgColorSwitchTimes = new();

    // --- timestep management ------------------------------------------------
    private const int UPDATE_STEP = 1;
    // private float pastTime = 0f;

    // --- coverage metric ----------------------------------------------------
    public const int N_GRIDS_X = 4;
    public const int N_GRIDS_Z = 4;
    [SerializeField] GameObject tile;

    // debug
    [SerializeField] TMP_Text debugTextfield; 


    void Update(){
        // only measure metrics when game is running
        if(GameManagement.gameState != GameState.Running
        && GameManagement.gameState != GameState.Training
        && GameManagement.gameState != GameState.VisualizePolicy) return;

        if(GameManagement.gameState == GameState.Training){
            UpdateCoverageMetric(false, Time.deltaTime*GameManagement.actionsPerSecond);
        }
        if(GameManagement.gameState == GameState.Running || GameManagement.gameState == GameState.VisualizePolicy){
            UpdateCoverageMetric(false, Time.deltaTime);
        }

        // only record metrics every UPDATE_STEP in seconds
        // if(pastTime < UPDATE_STEP && GameManagement.gameState != GameState.Training){
        //     pastTime  += Time.deltaTime;
        // } else {
        //     pastTime = 0;
        //     // --- TOTAL DIST TO SWARM CENTRE ---------------------------------
        //     UpdateAvgDistToCentre();
        //     // --- COVERAGE METRIC --------------------------------------------
        //     // UpdateCoverageMetric();
        //     // --- COLOR METRICS  ---------------------------------------------
        //     UpdateAverageNoColorTime();
        //     UpdateColorVisits();
        //     // --- SPEED METRIC -----------------------------------------------
        //     UpdateSpeed();
        // }
    }

    void Start(){
        // init csvs to write in
        Utilities.ExportArrayToCSV("0", "average no color time", "avgNoColorTime.csv");
        Utilities.ExportArrayToCSV("0", "average distance to swarm centre", "avgDistToCentre.csv");
        Utilities.ExportArrayToCSV("0", "average speed", "avgSpeed.csv");
        // Utilities.InitLocalMetrics();
        
        // generate header line for coverage metric
        String header = "";
        String initRow = "";
        for(int i = 0; i < N_GRIDS_X; i++){
            for(int j = 0; j < N_GRIDS_Z; j++){
                header += "gridpos" + i + ":" + j + ",";
                initRow += "0,";
        }}
        Utilities.ExportArrayToCSV(initRow, header, "coverage.csv");

        // init color visits
        for(int i = 0; i<4; i++){
            colorVisits[i] = 0f;
        }

        // init lists so we don't get empty list errors
        avgColorSwitchTimes.Add(0);

    }

    public List<float> GetSwarmMetrics(){
        List<float> obs = new();

        // add single numerical metrics
        obs.Add(UpdateSpeed(false));
        obs.Add(UpdateAvgDistToCentre(false));
        obs.Add(UpdateAverageNoColorTime(false));
        
        // add vector metrics
        foreach (var item in UpdateColorVisits(false))
        {
            obs.Add(item);
        }
        // coverage metric
        foreach (var item in lastVisitTime)
        {
            obs.Add(Mathf.Round(item));
        }

        // output
        print(obs);
        return obs;
    }

    public void ShowCoverage(){
        float step_x = (GameManagement.ARENA_X_MAX - GameManagement.ARENA_X_MIN) / MetricManagement.N_GRIDS_X;
        float step_z = (GameManagement.ARENA_Z_MAX - GameManagement.ARENA_Z_MIN) / MetricManagement.N_GRIDS_Z;
        int index_x = 0;

        // add tiles which indicate, how long it wasn't visited
        for(float x = GameManagement.ARENA_X_MIN + (step_x/2); x <= GameManagement.ARENA_X_MAX; x += step_x){
            int index_z = 0;
            for(float z = GameManagement.ARENA_Z_MIN+ (step_z/2); z <= GameManagement.ARENA_Z_MAX; z += step_z){
                GameObject inst = Instantiate(tile, new Vector3(x, 1, z), Quaternion.identity, transform);
                inst.GetComponent<VisualiseCoverage>().index_x = index_x;
                inst.GetComponent<VisualiseCoverage>().index_z = index_z;
                index_z++;
            }
            index_x++;
        }
    }

    float[] UpdateColorVisits(bool writeCSV = true){
        if(!writeCSV) return colorVisits;
        
        String row = "";
        for(int i = 0; i < 4; i++){
            row += colorVisits[i] + "\n";
        }
        Utilities.ExportArrayToCSV(row, "Appearance of color combinations", "colorVisits.csv");
        return colorVisits;
    }
    float UpdateAverageNoColorTime(bool writeCSV = true){
        avgColorSwitchTime = avgColorSwitchTimes.Average();
        if(!writeCSV) return avgColorSwitchTime;
        Utilities.AppendLineToFile("avgNoColorTime.csv", avgColorSwitchTime.ToString());
        return avgColorSwitchTime;
    }
    float UpdateSpeed(bool writeCSV = true){
        float totalSpeed = 0;
        int nBots = 0;
        GameManagement.allBots.ForEach((bot) => {
            totalSpeed += bot.GetComponent<ColorTracker>().realSpeed;
            nBots++;
        });

        avgSpeed = totalSpeed / nBots;
        if(!writeCSV) return avgSpeed;
        Utilities.AppendLineToFile("avgSpeed.csv", avgSpeed.ToString());
        return avgSpeed;
    }
    float[,] UpdateCoverageMetric(bool writeCSV = true, float deltaTime = 1){
        // update last visit time
        for (int xi = 0; xi < N_GRIDS_X; xi++)
        {
            for (int zi = 0; zi < N_GRIDS_Z; zi++)
            {
                lastVisitTime[xi,zi] += deltaTime;
            }
        }

        // reset every cell with a bot
        GameManagement.allBots.ForEach((bot) => {
            int cellX = (int)(bot.transform.position.x - GameManagement.ARENA_X_MIN);
            int cellZ = (int)(bot.transform.position.z - GameManagement.ARENA_Z_MIN);

            lastVisitTime[cellX, cellZ] = 0f;
        });

        // output to csv or only return depending on input parameter
        if(!writeCSV) return lastVisitTime;

        String row = "";
        for(int i = 0; i < N_GRIDS_X; i++){
            for(int j = 0; j < N_GRIDS_Z; j++){
                row += lastVisitTime[i, j] + ",";
        }}
        Utilities.AppendLineToFile("coverage.csv", row);
        return lastVisitTime;
    }
    float UpdateAvgDistToCentre(bool writeCSV = true){
        // calc swarm centre
        float meanX = 0;
        float meanZ = 0;
        GameManagement.allBots.ForEach((bot) => {
            meanX += bot.transform.position.x;
            meanZ += bot.transform.position.z;
        });
        meanX = meanX / GameManagement.allBots.Count;
        meanZ = meanZ / GameManagement.allBots.Count;
        Vector2 swarmCentre = new Vector3(meanX, meanZ);

        // calc total dist to centre
        Vector2 botPos;
        totalDistToCenter = 0;

        GameManagement.allBots.ForEach((bot) => {
            botPos = new Vector3(bot.transform.position.x , bot.transform.position.z);
            totalDistToCenter += Math.Abs(Vector2.Distance(botPos, swarmCentre));
        });

        // output to csv or to caller, depending on input parameter
        avgDistToCenter = totalDistToCenter / GameManagement.allBots.Count;
        if(!writeCSV) return avgDistToCenter;
        Utilities.AppendLineToFile("avgDistToCentre.csv", avgDistToCenter.ToString());
        return avgDistToCenter;
    }
}
