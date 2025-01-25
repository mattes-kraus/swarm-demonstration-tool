using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
        // Only visualise trail for one robot
        active = transform.parent.GetComponent<Turtlebot>().indexInAllBots == 0;

        if(active){
            // Finde Fixpunkt für die LineRenderer
            parent = GameObject.Find("Trail").transform;

            // Finde color tracker für echtes speed
            colorTracker = transform.parent.GetComponent<ColorTracker>();

            // Erstelle den ersten LineRenderer
            CreateNewLineRenderer();
        }
        
    }

    void Update()
    {
        // Only visualise trail for one robot
        if(active){
            // Update points in line renderer
            UpdateLineRenderer();

            // Change Color if speed changes
            if (Mathf.Abs(colorTracker.realSpeed - lastSpeed) > 0.01f)
            {
                ChangeColor(GetSpeedColor(colorTracker.realSpeed));
                lastSpeed = colorTracker.realSpeed;
            }
        }
    }

    private void CreateNewLineRenderer()
    {
        GameObject lineObject = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity, parent);
        currentLineRenderer = lineObject.GetComponent<LineRenderer>();
        if (currentLineRenderer == null)
        {
            Debug.LogError("Das Prefab benötigt einen LineRenderer!");
            return;
        }

        // Setze die aktuelle Farbe
        currentLineRenderer.startColor = currentColor;
        currentLineRenderer.endColor = currentColor;
        currentLineRenderer.positionCount = 1;

        // verhindere gap zwischen den Farben
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

    public void ChangeColor(Color newColor)
    {
        // Setze die neue Farbe
        currentColor = newColor;

        // Beende den aktuellen LineRenderer und starte einen neuen
        CreateNewLineRenderer();
    }
}
