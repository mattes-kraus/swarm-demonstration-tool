using System;
using System.Collections.Generic;

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