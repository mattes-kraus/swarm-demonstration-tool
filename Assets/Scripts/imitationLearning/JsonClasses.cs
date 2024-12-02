using System;
using System.Collections.Generic;

[Serializable]
public class ActionData
{
    public int orientation;
    public float speed;
}

[Serializable]
public class ObservationData
{
    public List<int> observations;
}

[Serializable]
public class ExecutedData
{
    public bool executed;
}