using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    [SerializeField] public ObjectType type;
     // Das Bild für den Mauszeiger, wenn er über dem UI-Element schwebt
    [SerializeField] private Texture2D cursorTexture;
    // if element is currently disabled for clicks, we want to propagate click to the ground

    void Start(){
        // For computing voronoi we keep track of walls. 
        // Currently they are not used but may be useful later
        if(type == ObjectType.Wall){
            VoronoiDiagram.walls.Add(gameObject);
        }
    }

    void Update(){
        // make object not clickable anymore when config of the arena is done
        if(GameManagement.gameState != GameState.Building && !(GameManagement.currentControlMode == ControlMode.ModifyBeacon && type == ObjectType.Beacon)){
            gameObject.layer = 2;
        } else {
            gameObject.layer = 0;
        }
    }

    // make visible when you can modify an object
    void OnMouseEnter()
    {
        if (GameManagement.gameState == GameState.Building 
        || GameManagement.currentControlMode == ControlMode.ModifyBeacon && type == ObjectType.Beacon){
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    // select this object to later be able to modify it from UI
    public void OnMouseDown()
    {
        GameManagement.ChangeSelectedObject(gameObject, type);
        GameManagement.StopAdding();
    }
}