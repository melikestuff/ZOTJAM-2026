using UnityEditor;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private BoxCollider2D cursor;
    [SerializeField] private BoxCollider2D oven;
    [SerializeField] private string ingredientType = "dough"; // "dough", "sauce", or "cheese"
    [SerializeField] private bool isPlaced = false;
    public Cursor cursorScript;
    public GameController gc;

    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();
        oven = GameObject.FindGameObjectWithTag("Plate").GetComponent<BoxCollider2D>();
        cursorScript = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Check if the ingredient is overlapping with the cursor collider
            if (cursor.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                // Only allow dragging if no other ingredient is being dragged, or this ingredient is already being dragged
                if (GameController.currentlyDraggedObject == null || GameController.currentlyDraggedObject == this)
                {
                    if (!isPlaced)
                    {
                        GameController.currentlyDraggedObject = this;

                        Vector3 mousePosition = Input.mousePosition;
                        mousePosition.z = 10;
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
                    }
                }
            }
        }

        // Clear the dragged ingredient reference when mouse is released
        if (Input.GetMouseButtonUp(0))
        {
            if (oven.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                // Check if prerequisites are met
                if (CanPlace())
                {
                    isPlaced = true;
                    transform.position = new Vector3(oven.transform.position.x, oven.transform.position.y, transform.position.z);
                    transform.SetParent(oven.transform);
                    UpdateGameController();
                }
            }

            GameController.currentlyDraggedObject = null;
        }
    }

    private bool CanPlace()
    {
        switch (ingredientType.ToLower())
        {
            case "dough":
                return true; // Dough can always be placed
            case "sauce":
                return gc.isDoughPlaced; // Sauce can only be placed if dough is placed
            case "cheese":
                return gc.isSauceAdded; // Cheese can only be placed if sauce is placed
            default:
                return false;
        }
    }

    private void UpdateGameController()
    {
        switch (ingredientType.ToLower())
        {
            case "dough":
                gc.isDoughPlaced = true;
                break;
            case "sauce":
                gc.isSauceAdded = true;
                break;
            case "cheese":
                gc.isCheeseAdded = true;
                break;
        }
    }
}
