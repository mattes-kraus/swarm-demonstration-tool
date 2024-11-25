using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MetricManagement: MonoBehaviour
{
    // --- actual metrics ------------------
    public float avgDistToCenter;
    public float avgSpeed;
    public float[,] lastVisitTime = new float[N_GRIDS_X, N_GRIDS_Z];
    public float avgColorSwitchTime;


    // --- color metrics --------------------------------
    // whitewhite:0, whiteblack:1, blackwhite:2, blackblack:3
    public int[] colorVisits = new int[4];
    
    // -- metric helpers --------------------------------
    private float totalDistToCenter;
    public List<float> avgColorSwitchTimes = new();

    // --- timestep management --------------------------
    private const int UPDATE_STEP = 1;
    private float pastTime = 0f;

    // --- coverage metric ------------------------------
    public const int N_GRIDS_X = 4;
    public const int N_GRIDS_Z = 4;
    [SerializeField] GameObject tile;

    void Update(){
        // only measure metrics when game is running
        if(GameManagement.gameState != GameState.Running && GameManagement.gameState != GameState.Training) return;

        // only record metrics every UPDATE_STEP in seconds
        if(pastTime < UPDATE_STEP && GameManagement.gameState != GameState.Training){
            pastTime  += Time.deltaTime;
        } else {
            pastTime = 0;
            // --- TOTAL DIST TO SWARM CENTRE ---------------------------------
            UpdateAvgDistToCentre();
            // --- COVERAGE METRIC --------------------------------------------
            UpdateCoverageMetric();
            // --- COLOR METRICS  ---------------------------------------------
            UpdateAverageNoColorTime();
            UpdateColorVisits();
            // --- SPEED METRIC -----------------------------------------------
            UpdateSpeed();
        }
    }

    void Start(){
        // init csvs to write in
        Utilities.ExportArrayToCSV("0", "average no color time", "avgNoColorTime.csv");
        Utilities.ExportArrayToCSV("0", "average distance to swarm centre", "avgDistToCentre.csv");
        Utilities.ExportArrayToCSV("0", "average speed", "avgSpeed.csv");
        
        // generate header line for coverage metric
        String header = "";
        String initRow = "";
        for(int i = 0; i < N_GRIDS_X; i++){
            for(int j = 0; j < N_GRIDS_Z; j++){
                header += "gridpos" + i + ":" + j + ",";
                initRow += "0,";
        }}
        Utilities.ExportArrayToCSV(initRow, header, "coverage.csv");

        // init lists so we don't get empty list errors
        avgColorSwitchTimes.Add(0);

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

    void UpdateColorVisits(){
        String row = "";
        for(int i = 0; i < 4; i++){
            row += colorVisits[i] + "\n";
        }
        Utilities.ExportArrayToCSV(row, "Appearance of color combinations", "colorVisits.csv");
    }
    void UpdateAverageNoColorTime(){
        avgColorSwitchTime = avgColorSwitchTimes.Average();
        Utilities.AppendLineToFile("avgNoColorTime.csv", avgColorSwitchTime.ToString());
    }
    void UpdateSpeed(){
        float totalSpeed = 0;
        int nBots = 0;
        GameManagement.allBots.ForEach((bot) => {
            totalSpeed += bot.GetComponent<ColorTracker>().realSpeed;
            nBots++;
        });

        avgSpeed = totalSpeed / nBots;
        Utilities.AppendLineToFile("avgSpeed.csv", avgSpeed.ToString());
    }
    void UpdateCoverageMetric(){
        // update last visit time
        for (int xi = 0; xi < N_GRIDS_X; xi++)
        {
            for (int zi = 0; zi < N_GRIDS_Z; zi++)
            {
                lastVisitTime[xi,zi] += 1;
            }
        }

        // reset every cell with a bot
        GameManagement.allBots.ForEach((bot) => {
            int cellX = (int)(bot.transform.position.x - GameManagement.ARENA_X_MIN);
            int cellZ = (int)(bot.transform.position.z - GameManagement.ARENA_Z_MIN);

            lastVisitTime[cellX, cellZ] = 0;
        });

        String row = "";
        for(int i = 0; i < N_GRIDS_X; i++){
            for(int j = 0; j < N_GRIDS_Z; j++){
                row += lastVisitTime[i, j] + ",";
        }}
        Utilities.AppendLineToFile("coverage.csv", row);

        // print coverage to the console
        // Utilities.Stringify2DArray(lastVisitTime, N_GRIDS_X, N_GRIDS_Z);
    }
    void UpdateAvgDistToCentre(){
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

        avgDistToCenter = totalDistToCenter / GameManagement.allBots.Count;
        Utilities.AppendLineToFile("avgDistToCentre.csv", avgDistToCenter.ToString());
        // Debug.Log("total dist to centre: " + totalDistToCenter);
    }
}
