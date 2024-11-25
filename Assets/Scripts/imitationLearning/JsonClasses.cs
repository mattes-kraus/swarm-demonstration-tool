using System;
using System.Collections.Generic;

[Serializable]
public class ActionData
{
    public int orientation;
    public float speed;
}

[Serializable]
public class Root
{
    public int iteration;
    public List<ActionData> action_data;
}

[Serializable]
public class ObservationData
{
    public bool read;
}