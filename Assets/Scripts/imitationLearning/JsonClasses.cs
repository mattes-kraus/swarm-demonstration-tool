using System;
using System.Collections.Generic;


// JSON Structures for the in- and export of metrics and actions
[Serializable]
public class ActionData
{
    public float orientation;
    public float speed;
}

[Serializable]
public class ObservationData
{
    public List<float> observations;
    public bool terminated;
}

[Serializable]
public class ExecutedData
{
    public bool executed;
    public bool reset;
}

[Serializable]
public class Trajectory
{
    public List<List<float>> obs;
    public List<List<float>> acts;
}

[Serializable]
public class SwarmMetrics
{
    public float avgSpeed;
    public float avgDistToCenter;
    public float avgColorSwitchTime;
    public List<float> colorVisits;
}