using System;
using System.Collections.Generic;
using UnityEngine;

public class MetricManagement: MonoBehaviour
{
    // --- actual metrics ------------------
    public float totalDistToCenter;
    public float[,] lastVisitTime = new float[N_GRIDS_X, N_GRIDS_Z];

    
    // --- helper variables ----------------
    public List<Turtlebot> allBots = new();

    // --- timestep management -------------
    private const int UPDATE_STEP = 1;
    private float pastTime = 0f;

    // --- coverage metric -----------------
    private const int N_GRIDS_X = 4;
    private const int N_GRIDS_Z = 4;

    void Update(){
        // only measure metrics when game is running
        if(GameManagement.gameState != GameState.Running) return;

        // only record metrics every UPDATE_STEP in seconds
        if(pastTime < UPDATE_STEP){
            pastTime  += Time.deltaTime;
        } else {
            // --- TOTAL DIST TO SWARM CENTRE ---------------------------------
            // calc swarm centre
            float meanX = 0;
            float meanZ = 0;
            allBots.ForEach((bot) => {
                meanX += bot.transform.position.x;
                meanZ += bot.transform.position.z;
            });
            meanX = meanX / allBots.Count;
            meanZ = meanZ / allBots.Count;
            Vector2 swarmCentre = new Vector3(meanX, meanZ);

            // calc total dist to centre
            Vector2 botPos;
            totalDistToCenter = 0;

            allBots.ForEach((bot) => {
                botPos = new Vector3(bot.transform.position.x , bot.transform.position.z);
                totalDistToCenter += Math.Abs(Vector2.Distance(botPos, swarmCentre));
            });

            Debug.Log("total dist to centre: " + totalDistToCenter);
            pastTime = 0;

            // --- COVERAGE METRIC --------------------------------------------
            // update last visit time
            for (int xi = 0; xi < N_GRIDS_X; xi++)
            {
                for (int zi = 0; zi < N_GRIDS_Z; zi++)
                {
                    lastVisitTime[xi,zi] += 1;
                }
            }

            // reset every cell with a bot
            allBots.ForEach((bot) => {
                int cellX = (int)(bot.transform.position.x - GameManagement.ARENA_X_MIN);
                int cellZ = (int)(bot.transform.position.z - GameManagement.ARENA_Z_MIN);

                lastVisitTime[cellX, cellZ] = 0;
            });

            Utilities.Stringify2DArray(lastVisitTime, N_GRIDS_X, N_GRIDS_Z);

            // --- COLOR METRIC  ----------------------------------------------
        }
    }
}
