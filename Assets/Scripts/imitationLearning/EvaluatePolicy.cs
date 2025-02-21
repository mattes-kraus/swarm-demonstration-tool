using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


// Visualizes the trail of the robot
public class EvaluatePolicy : MonoBehaviour
{
    private bool active;
    [SerializeField] private GameObject lineRendererPrefab; // Ein Prefab mit einem LineRenderer
    private Transform parent;
    private ColorTracker colorTracker;
    private float lastSpeed = 0;
    public Color currentColor = Color.white;
    private LineRenderer currentLineRenderer;

    private List<Vector3> positions = new List<Vector3>(); // Positionen für den aktuellen LineRenderer


    void Start()
    {
        // Evaluate arguments the simulation was started with
        switch(GameManagement.trail_visu){
            // no trail
            case 0:
                active = false;
                break;
            // trail for one robot
            case 1:
                active = transform.parent.GetComponent<Turtlebot>().indexInAllBots == 0;
                break;
            // trail for all robots
            case 2:
                active = true;
                break;
            default:
                active = false;
                break;
        }

        if(active){
            // Put LineRenderer in Trail GameObject (position 0,0,0) so 
            // I can use absolute positions for the trail
            parent = GameObject.Find("Trail").transform;
            colorTracker = transform.parent.GetComponent<ColorTracker>();

            // set color once and create one linerenderer for each robot
            if(GameManagement.trail_visu == 2){
                currentColor = GetRobotColor();
            }

            CreateNewLineRenderer();    
        }
        
    }

    void Update()
    {
        // Update trail
        if(active){
            // Update points in line renderer
            UpdateLineRenderer();

            // If we visualize one robot, the color is dependet from current speed. 
            // Therefore change Color if speed changes
            if(GameManagement.trail_visu == 1 
            && Mathf.Abs(colorTracker.realSpeed - lastSpeed) > 0.01f){
                ChangeColor(GetSpeedColor(colorTracker.realSpeed));
                lastSpeed = colorTracker.realSpeed;
            }
        }
    }   

    private void CreateNewLineRenderer()
    {
        GameObject lineObject = Instantiate(
            lineRendererPrefab, 
            Vector3.zero, 
            Quaternion.identity, parent
        );
        currentLineRenderer = lineObject.GetComponent<LineRenderer>();
        if (currentLineRenderer == null)
        {
            Debug.LogError("Das Prefab benötigt einen LineRenderer!");
            return;
        }

        // Set current color
        currentLineRenderer.startColor = currentColor;
        currentLineRenderer.endColor = currentColor;
        currentLineRenderer.positionCount = 1;

        // If we start the new colored trail with the last position of the old 
        // trail, we prevent gaps between the trails
        Vector3 firstPoint = Vector3.zero;
        if(positions.Count > 0)
            firstPoint = positions[positions.Count - 1];
        positions.Clear();
        if(firstPoint != Vector3.zero)
            positions.Add(firstPoint);
    }

    private void UpdateLineRenderer()
    {
        // Füge die aktuelle Position hinzu
        Vector3 currentPosition = transform.position;
        if (positions.Count == 0 || Vector3.Distance(positions[positions.Count - 1], currentPosition) > 0.01f)
        {
            positions.Add(currentPosition);
            currentLineRenderer.positionCount = positions.Count;
            currentLineRenderer.SetPositions(positions.ToArray());
        }
    }

     public Color GetSpeedColor(float speed)
    {
        // Clamp speed to be within 0 and 0.31
        speed = Mathf.Clamp(speed, 0f, 0.31f);

        // Normalize speed to be between 0 and 1
        float normalizedSpeed = speed / 0.31f;

        // Define the colors
        Color slowColor = Color.blue;//new(0.74f, 0.89f, 1, 1); // Represents slow speed
        Color fastColor = new(1, 0.89f, 0, 1); // Represents fast speed

        // Lerp between the colors
        return Color.Lerp(slowColor, fastColor, normalizedSpeed);
    }

    public Color GetRobotColor()
    {
        int index = transform.parent.GetComponent<Turtlebot>().indexInAllBots;

        // university of konstanz colors
        switch(index){
            case 0:
                return new(89 / 255f, 199 / 255f, 235 / 255f);
            case 1:
                return new(254 / 255f, 160 / 255f, 144 / 255f);
            case 2:
                return new(154 / 255f, 160 / 255f, 167 / 255f);
            case 3:
                return new(7 / 255f, 113 / 255f, 135 / 255f);
            case 4:
                return new(10 / 255f, 144 / 255f, 134 / 255f);
            case 5:
                return new(224 / 255f, 96 / 255f, 126 / 255f);
            default:
                return new(89 / 255f, 199 / 255f, 235 / 255f);
        };
    }

    public void ChangeColor(Color newColor)
    {
        // Set new color
        currentColor = newColor;

        // Stop old line renderer and create new one
        CreateNewLineRenderer();
    }
}
