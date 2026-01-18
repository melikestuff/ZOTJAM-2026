using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private BoxCollider2D cursor;
    [SerializeField] private BoxCollider2D oven;
    [SerializeField] private bool isPlaced = false;
    [SerializeField] private bool isInOven = false;
    public Cursor cursorScript;
    public GameController gc;

    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();
        oven = GameObject.FindGameObjectWithTag("Oven").GetComponent<BoxCollider2D>();
        cursorScript = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Check if the plate is overlapping with the cursor collider
            if (cursor.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                // Only allow dragging if no other object is being dragged or this plate is already being dragged
                if (GameController.currentlyDraggedObject == null || GameController.currentlyDraggedObject == this)
                {
                    GameController.currentlyDraggedObject = this;

                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = 10;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
                }
            }
        }

        // Clear the dragged object reference when mouse is released
        if (Input.GetMouseButtonUp(0))
        {
            // Check if plate is placed in oven
            if (oven.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                if (!gc.isOvenCooking)
                {
                    isInOven = true;
                    isPlaced = true;
                    transform.position = new Vector3(oven.transform.position.x, oven.transform.position.y, transform.position.z);
                    
                    // Start plate timer
                    gc.isOvenCooking = true;
                    gc.plateTimer = gc.plateCookTime;
                }
            }
            else
            {
                // Not in oven, reset position
                if (!isInOven)
                {
                    isPlaced = false;
                    transform.position = new Vector3(0.88f, -2.79f, 0);
                }
            }

            GameController.currentlyDraggedObject = null;
        }
        else
        {
            // Keep the plate on the starting position if not cooking
            if (!gc.isOvenCooking)
            {
                isInOven = false;
            }
        }
    }
}
