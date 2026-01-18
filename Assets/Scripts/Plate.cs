using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private BoxCollider2D cursor;
    [SerializeField] private BoxCollider2D oven;
    [SerializeField] private bool isInOven = false;
    public Cursor cursorScript;
    public GameController gc;

    // Track cooking state and ingredients on this plate
    public bool IsCooked { get; private set; } = false;
    private readonly List<string> ingredients = new List<string>();

    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();
        oven = GameObject.FindGameObjectWithTag("Oven").GetComponent<BoxCollider2D>();
        cursorScript = GameObject.FindGameObjectWithTag("Cursor").GetComponent<Cursor>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        // Log initial ingredient state
        LogIngredients();
    }

    void Update()
    {
        // If this plate is in the oven while the oven is cooking, lock dragging until cooking finishes.
        bool ovenLocked = isInOven && gc != null && gc.isOvenCooking;

        if (Input.GetMouseButton(0))
        {
            // Check if the plate is overlapping with the cursor collider
            if (cursor.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                // If the plate is locked in the oven, do not allow dragging it out.
                if (ovenLocked)
                {
                    // optional debug to help diagnose attempts to drag locked plate
                    // Debug.Log($"{name} is locked in oven until cooking completes.");
                }
                else
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
        }

        // Clear the dragged object reference when mouse is released
        if (Input.GetMouseButtonUp(0))
        {
            // FIRST: check for trash zone (destroy plate + children)
            var trashObj = GameObject.FindGameObjectWithTag("Trash");
            if (trashObj != null)
            {
                BoxCollider2D trashCol = trashObj.GetComponent<BoxCollider2D>();
                if (trashCol != null && trashCol.bounds.Intersects(GetComponent<Collider2D>().bounds))
                {
                    Destroy(gameObject); // destroys plate and all children
                    GameController.currentlyDraggedObject = null;
                    return;
                }
            }

            // SECOND: check for drop-off zone
            var dropOffObj = GameObject.FindGameObjectWithTag("DropOff");
            if (dropOffObj != null)
            {
                BoxCollider2D dropOffCol = dropOffObj.GetComponent<BoxCollider2D>();
                if (dropOffCol != null && dropOffCol.bounds.Intersects(GetComponent<Collider2D>().bounds))
                {
                    // Only consider delivering cooked plates
                    if (IsCooked)
                    {
                        // Ask GameController for matching sprite
                        Sprite matched = gc.GetMatchingPizzaSprite(this);
                        if (matched != null)
                        {
                            // Remove the first matching UI entry
                            bool removed = gc.RemovePizzaListEntryBySprite(matched);
                            Debug.Log($"Plate delivered matched pizza sprite: {matched.name}. Removed from list: {removed}");
                        }
                        else
                        {
                            Debug.Log("Plate delivered did not match any pizza on the mapping (no removal).");
                        }

                        // Destroy plate after successful delivery
                        GameController.currentlyDraggedObject = null;
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        Debug.Log("Plate dropped on DropOff but is not cooked yet.");
                        GameController.currentlyDraggedObject = null;
                        return;
                    }
                }
            }

            // Check if plate is placed in oven
            if (oven.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                // Do not allow cooked plates to be placed back into the oven
                if (IsCooked)
                {
                    Debug.Log($"{name} is already cooked and cannot be placed back into the oven.");
                    // snap back to start so it doesn't remain overlapping oven
                    if (!isInOven)
                        transform.position = new Vector3(3.3f, -3.5f, 0);
                }
                else if (!gc.isOvenCooking)
                {
                    isInOven = true;
                    transform.position = new Vector3(oven.transform.position.x, oven.transform.position.y, transform.position.z);

                    // Start plate timer on GameController and register this plate as the current cooking plate
                    gc.isOvenCooking = true;
                    gc.plateTimer = gc.plateCookTime;
                    gc.currentCookingPlate = this;
                }
            }
            else
            {
                // Not in oven, reset position
                if (!isInOven)
                {
                    transform.position = new Vector3(3.3f, -3.5f, 0);
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

    // Public API for tracking ingredients on this plate
    public bool HasIngredient(string ingredientType)
    {
        if (string.IsNullOrEmpty(ingredientType))
            return false;
        return ingredients.Contains(ingredientType.ToLower());
    }

    public void AddIngredient(string ingredientType)
    {
        if (string.IsNullOrEmpty(ingredientType))
            return;

        string key = ingredientType.ToLower();
        if (!ingredients.Contains(key))
            ingredients.Add(key);

        // Log current ingredient list after change
        LogIngredients();
    }

    public bool RemoveIngredient(string ingredientType)
    {
        if (string.IsNullOrEmpty(ingredientType))
            return false;

        bool removed = ingredients.Remove(ingredientType.ToLower());

        // Log current ingredient list after change
        LogIngredients();

        return removed;
    }

    public IReadOnlyList<string> GetIngredients() => ingredients.AsReadOnly();

    // Helper to log the ingredient list for debugging
    private void LogIngredients()
    {
        if (ingredients.Count == 0)
        {
            Debug.Log($"{name} Ingredients: <empty>");
        }
        else
        {
            Debug.Log($"{name} Ingredients: {string.Join(", ", ingredients)}");
        }
    }

    // Called by GameController when cooking completes for this plate
    public void SetCooked(bool cooked)
    {
        // Avoid reapplying tint if state already matches
        if (IsCooked == cooked)
            return;

        IsCooked = cooked;

        if (cooked)
        {
            // Find any dough ingredient child and apply a very slight dark tint.
            foreach (Transform child in transform)
            {
                if (child == null)
                    continue;

                var ing = child.GetComponent<Ingredient>();
                if (ing != null && ing.IngredientType == "dough")
                {
                    // 0.95 => 5% darker (very slight)
                    ing.ApplyCookedTint(0.7f);
                }
            }
        }
    }
}
