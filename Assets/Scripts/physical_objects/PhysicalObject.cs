using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    [SerializeField] public ObjectType type;
     // Das Bild für den Mauszeiger, wenn er über dem UI-Element schwebt
    [SerializeField] private Texture2D cursorTexture;
    // if element is currently disabled for clicks, we want to propagate click to the ground
    private ModificationAdder underlyingElement;

    void Start(){
        // if we press a groundsticker maybe we wanted to spawn something
        // therefore in some cases we redirect the click to the ground
        underlyingElement = GameObject.Find("Ground").GetComponent<ModificationAdder>();

        // for computing voronoi, we need the walls
        if(type == ObjectType.Wall){
            GameObject.Find("VoronoiVis").GetComponent<VoronoiDiagram>().walls.Add(gameObject);
        }
    }

    void Update(){
        // make object not clickable anymore when config of the arena is done
        if(GameManagement.gameState == GameState.Running && type != ObjectType.Beacon){
            gameObject.layer = 2;
        } 
    }

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
    
    public void OnMouseDown()
    {
        if (GameManagement.gameState == GameState.Building 
        || (GameManagement.currentControlMode == ControlMode.ModifyBeacon && type == ObjectType.Beacon)){
            Debug.Log("physical object clicked");
        // select this object to later be able to modify it from UI
            GameManagement.ChangeSelectedObject(gameObject, type);
            GameManagement.StopAdding();
        } else {
        // redirect mouseclick to the ground if we aren't building anymore
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo);

            underlyingElement.SetBehaviorPos(hitInfo.point.x, hitInfo.point.z);
            Vector3 point = hitInfo.point;
            point.y = 0.53f;
            underlyingElement.AddNewElement(point);
        }
    }
}