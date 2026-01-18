using UnityEditor;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField] private BoxCollider2D cursor;
    [SerializeField] private BoxCollider2D plate; // kept for compatibility but not relied on for drop detection
    [SerializeField] private string ingredientType = "dough"; // "dough", "sauce", "cheese", "pineapple", "mushroom", "basil", "pepperoni"
    [SerializeField] public bool isPlaced = false;
    
    public Cursor cursorScript;
    public GameController gc;

    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();
        // Don't assume a single plate found at Start; plates may be instantiated later.
        // Keep a reference if a plate exists in scene, but we will locate the plate at drop time.
        var plateObj = GameObject.FindGameObjectWithTag("Plate");
        if (plateObj != null)
            plate = plateObj.GetComponent<BoxCollider2D>();

        cursorScript = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Check if the ingredient is overlapping with the cursor collider
            if (cursor != null && cursor.bounds.Intersects(GetComponent<Collider2D>().bounds))
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
            // Find the plate currently under this ingredient (robust to multiple plates / instantiation timing)
            Transform targetPlate = FindPlateUnderPosition(transform.position);
            if (targetPlate != null)
            {
                // Check if prerequisites are met
                if (CanPlace())
                {
                    // If an ingredient of same type already exists on plate, destroy this duplicate (do not add)
                    if (IsDuplicateOnPlate(targetPlate))
                    {
                        GameController.currentlyDraggedObject = null;
                        Destroy(gameObject);
                        return;
                    }

                    isPlaced = true;

                    // Parent to the specific plate and place at the plate center (no offset)
                    transform.SetParent(targetPlate, worldPositionStays: false);

                    // Place exactly at plate local origin (preserve current z)
                    transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);

                    UpdateGameController();
                }
            }

            GameController.currentlyDraggedObject = null;
        }
    }

    // Finds a plate transform under a given world position. Uses a tiny overlap check so it reliably finds a
    // plate collider even if the ingredient's center isn't perfectly on the plate.
    private Transform FindPlateUnderPosition(Vector2 worldPos)
    {
        // small radius to catch the plate collider under the ingredient
        const float checkRadius = 0.05f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, checkRadius);
        foreach (var c in hits)
        {
            if (c != null && c.CompareTag("Plate"))
                return c.transform;
        }

        // fallback: if plate field was found at Start, use that
        if (plate != null)
            return plate.transform;

        return null;
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

            // toppings: only placeable after cheese is added
            case "pineapple":
            case "mushroom":
            case "basil":
            case "pepperoni":
                return gc.isCheeseAdded;

            default:
                return false;
        }
    }

    // Check duplicates against the specific plate transform passed in (safer than using a plate found at Start)
    private bool IsDuplicateOnPlate(Transform plateTransform)
    {
        if (plateTransform == null)
            return false;

        foreach (Transform child in plateTransform)
        {
            if (child == null)
                continue;

            // skip self just in case (shouldn't be parented yet)
            if (child == transform)
                continue;

            Ingredient childIngredient = child.GetComponent<Ingredient>();
            if (childIngredient == null)
                continue;

            // Compare ingredient types (case-insensitive). If an identical ingredient is already placed, treat as duplicate.
            if (!string.IsNullOrEmpty(childIngredient.ingredientType) &&
                childIngredient.ingredientType.ToLower() == ingredientType.ToLower() &&
                childIngredient.isPlaced)
            {
                return true;
            }
        }

        return false;
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
