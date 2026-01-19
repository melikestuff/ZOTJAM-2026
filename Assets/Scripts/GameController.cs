using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Transform parentObj;
    public bool isDoughPlaced = false;
    public bool isSauceAdded = false;
    public bool isCheeseAdded = false;
    public GameObject plateProgress1;
    [SerializeField] private GameObject plateSpawner;
    [SerializeField] private GameObject sauceSpawner;
    [SerializeField] private GameObject cheeseSpawner;
    [SerializeField] private GameObject doughSpawner;
    [SerializeField] private GameObject PineappleSpawner;
    [SerializeField] private GameObject MushroomSpawner;
    [SerializeField] private GameObject BasilSpawner;
    [SerializeField] private GameObject PepperoniSpawner;
    [SerializeField] private GameObject platePrefab;
    [SerializeField] private GameObject saucePrefab;
    [SerializeField] private GameObject cheesePrefab;
    [SerializeField] private GameObject doughPrefab;
    [SerializeField] private GameObject pineapplePrefab;
    [SerializeField] private GameObject mushroomPrefab;
    [SerializeField] private GameObject basilPrefab;
    [SerializeField] private GameObject pepperoniPrefab;
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private GameObject uiPizzaPrefab;
    [SerializeField] private GameObject PizzaListPrefab;
    [SerializeField] public GameObject HorizontalLayoutParent; // made public so other scripts can access
    [SerializeField] public GameObject Bubble;

    public Transform customerSpawnPoint;
    public Transform bubblePizzaSpawnPoint;

    //customer sprites
    //general male sprites
    public Sprite gen1;
    public Sprite gen2;
    public Sprite gen3;
    public Sprite gen4;
    public Sprite gen5;
    public Sprite gen6;
    public Sprite gen7;
    public Sprite gen8;
    public Sprite gen9;
    public Sprite gen10;
    public Sprite gen11;
    public Sprite gen12;
    public Sprite gen13;
    public Sprite gen14;
    public Sprite gen15;
    public Sprite gen16;
    public Sprite gen17;
    public Sprite gen18;
    public Sprite gen19;
    public Sprite gen20;
    public Sprite gen21;

    public Sprite pizza1;
    public Sprite pizza2;
    public Sprite pizza3;
    public Sprite pizza4;
    public Sprite pizza5;
    public Sprite pizza6;
    public Sprite pizza7;
    public Sprite pizza8;

    public static MonoBehaviour currentlyDraggedObject = null;

    public bool isOvenCooking = false;
    public float plateTimer = 0f;
    public float plateCookTime = 5f; // Time in seconds for cooking

    private GameObject customer = null;
    private SpriteRenderer customerSpriteRenderer = null;
    private float spriteChangeTimer = 0f;
    private float spriteChangeInterval = 15f; // Change sprite xseconds

    // UI pizza (bubble) instances & timers
    private GameObject uiPizza = null;
    private SpriteRenderer uiPizzaSpriteRenderer = null;
    private Image uiPizzaImage = null;
    private float uiSpriteChangeTimer = 0f;

    // Track the Plate currently in the oven so we can mark it cooked when timer ends
    public Plate currentCookingPlate = null;

    // Track active plates: only allow 1 uncooked plate that is NOT in the oven
    private Plate activeUncookedPlate = null;

    void Start()
    {
        plateSpawner = GameObject.FindGameObjectWithTag("PlateSpawner");
        sauceSpawner = GameObject.FindGameObjectWithTag("SauceSpawner");
        cheeseSpawner = GameObject.FindGameObjectWithTag("CheeseSpawner");
        doughSpawner = GameObject.FindGameObjectWithTag("DoughSpawner");
        PineappleSpawner = GameObject.FindGameObjectWithTag("PineappleSpawner");
        MushroomSpawner = GameObject.FindGameObjectWithTag("MushroomSpawner");
        BasilSpawner = GameObject.FindGameObjectWithTag("BasilSpawner");
        PepperoniSpawner = GameObject.FindGameObjectWithTag("PepperoniSpawner");

        // Spawn customer prefab with random sprite gen1-10 at customer spawner position
        if (customerPrefab != null && customerSpawnPoint != null)
        {
            customer = Instantiate(customerPrefab, customerSpawnPoint.position, Quaternion.identity, parentObj);
            customerSpriteRenderer = customer.GetComponent<SpriteRenderer>();

            if (customerSpriteRenderer != null)
            {
                ChangeCustomerSprite();
                spriteChangeTimer = spriteChangeInterval;
            }
            else
            {
                Debug.LogWarning("Customer prefab does not have a SpriteRenderer component");
            }
        }
        else
        {
            if (customerPrefab == null)
                Debug.LogWarning("Customer prefab is not assigned");
            if (customerSpawnPoint == null)
                Debug.LogWarning("Customer spawn point is not assigned");
        }

        // Spawn UI pizza prefab at the bubble pizza spawn point on start and set up sprite cycling
        CreateUIPizzaBubble();
    }

    void Update()
    {
        HandleIngredientSpawning(doughSpawner, doughPrefab);
        HandleIngredientSpawning(sauceSpawner, saucePrefab);
        HandleIngredientSpawning(cheeseSpawner, cheesePrefab);

        // toppings: same instantiation logic as other ingredients
        HandleIngredientSpawning(PineappleSpawner, pineapplePrefab);
        HandleIngredientSpawning(MushroomSpawner, mushroomPrefab);
        HandleIngredientSpawning(BasilSpawner, basilPrefab);
        HandleIngredientSpawning(PepperoniSpawner, pepperoniPrefab);

        HandlePlateSpawning(plateSpawner, platePrefab);

        // Update customer sprite on its timer
        if (customerSpriteRenderer != null)
        {
            spriteChangeTimer -= Time.deltaTime;
            if (spriteChangeTimer <= 0f)
            {
                ChangeCustomerSprite();
                spriteChangeTimer = spriteChangeInterval;
            }
        }

        // Update UI pizza sprite on its timer
        if (uiPizzaSpriteRenderer != null || uiPizzaImage != null)
        {
            uiSpriteChangeTimer -= Time.deltaTime;
            if (uiSpriteChangeTimer <= 0f)
            {
                ChangeUIPizzaSprite();
                uiSpriteChangeTimer = spriteChangeInterval;
            }
        }

        // Update plate timer
        if (isOvenCooking)
        {
            plateTimer -= Time.deltaTime;

            // Update progress bar
            if (plateProgress1 != null)
            {
                float progress = (1f - (plateTimer / plateCookTime)) * 0.7f;
                plateProgress1.transform.localScale = new Vector3(progress, 0.2f, 1f);
            }

            // Check if cooking is done
            if (plateTimer <= 0f)
            {
                isOvenCooking = false;
                plateTimer = 0f;

                // Mark the plate in the oven as cooked
                if (currentCookingPlate != null)
                {
                    currentCookingPlate.SetCooked(true);
                    // Clear active uncooked plate since it's now cooked and in the oven
                    activeUncookedPlate = null;
                    currentCookingPlate = null;
                }

                // Update progress bar to full
                if (plateProgress1 != null)
                {
                    plateProgress1.transform.localScale = new Vector3(0.7f, 0.2f, 1f);
                }
            }
        }
    }

    private void ChangeCustomerSprite()
    {
        if (customerSpriteRenderer == null)
            return;

        // Ensure customer (and bubble container if assigned) becomes visible when we change sprite on the timer
        customerSpriteRenderer.enabled = true;
        if (Bubble != null)
            Bubble.SetActive(true);

        // expanded range: gen1 .. gen21
        Sprite[] customerSprites = {
            gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, gen10,
            gen11, gen12, gen13, gen14, gen15, gen16, gen17, gen18, gen19, gen20, gen21
        };

        int randomIndex = Random.Range(0, customerSprites.Length);
        customerSpriteRenderer.sprite = customerSprites[randomIndex];
    }

    private void ChangeUIPizzaSprite()
    {
        // Make UI pizza visible again when we change sprite on the timer (and ensure bubble active)
        if (uiPizzaSpriteRenderer != null)
            uiPizzaSpriteRenderer.enabled = true;
        else if (uiPizzaImage != null)
            uiPizzaImage.enabled = true;

        if (Bubble != null)
            Bubble.SetActive(true);

        Sprite[] pizzaSprites = { pizza1, pizza2, pizza3, pizza4, pizza5, pizza6, pizza7, pizza8 };
        int randomIndex = Random.Range(0, pizzaSprites.Length);
        Sprite chosen = pizzaSprites[randomIndex];

        if (uiPizzaSpriteRenderer != null)
        {
            uiPizzaSpriteRenderer.sprite = chosen;
        }
        else if (uiPizzaImage != null)
        {
            uiPizzaImage.sprite = chosen;
        }

        // For every new ui pizza (i.e. whenever the bubble's sprite changes), create a list entry
        CreatePizzaListEntry(chosen);
    }

    // Creates the UI pizza bubble instance and initializes sprite/timer
    private void CreateUIPizzaBubble()
    {
        if (uiPizzaPrefab == null || bubblePizzaSpawnPoint == null)
        {
            if (uiPizzaPrefab == null)
                Debug.LogWarning("UI pizza prefab is not assigned");
            if (bubblePizzaSpawnPoint == null)
                Debug.LogWarning("Bubble pizza spawn point is not assigned");
            return;
        }

        uiPizza = Instantiate(uiPizzaPrefab, bubblePizzaSpawnPoint.position, Quaternion.identity, parentObj);

        // Try to get a SpriteRenderer (for world-space sprite) or a UI.Image (for UI element)
        uiPizzaSpriteRenderer = uiPizza.GetComponent<SpriteRenderer>();
        if (uiPizzaSpriteRenderer == null)
        {
            uiPizzaImage = uiPizza.GetComponent<Image>();
            // If the prefab has nested Image component (not on root), try to find it in children
            if (uiPizzaImage == null)
                uiPizzaImage = uiPizza.GetComponentInChildren<Image>();
        }

        if (uiPizzaSpriteRenderer != null || uiPizzaImage != null)
        {
            ChangeUIPizzaSprite();
            uiSpriteChangeTimer = spriteChangeInterval;
        }
        else
        {
            Debug.LogWarning("UI pizza prefab does not have a SpriteRenderer or Image component");
        }
    }

    // Instantiates PizzaListPrefab as a child of HorizontalLayoutParent and sets its Image to the provided sprite.
    private void CreatePizzaListEntry(Sprite sprite)
    {
        if (PizzaListPrefab == null || HorizontalLayoutParent == null)
        {
            if (PizzaListPrefab == null)
                Debug.LogWarning("PizzaListPrefab is not assigned");
            if (HorizontalLayoutParent == null)
                Debug.LogWarning("HorizontalLayoutParent is not assigned");
            return;
        }

        GameObject listEntry = Instantiate(PizzaListPrefab, parentObj);
        // Parent under HorizontalLayoutParent while preserving local layout (useWorldPositionStays = false)
        listEntry.transform.SetParent(HorizontalLayoutParent.transform, false);

        // Try to find an Image component on the prefab or its children
        Image entryImage = listEntry.GetComponent<Image>();
        if (entryImage == null)
            entryImage = listEntry.GetComponentInChildren<Image>();

        if (entryImage != null)
        {
            entryImage.sprite = sprite;
            // If needed, set native size so layout respects sprite dimensions
            //entryImage.SetNativeSize();
        }
        else
        {
            Debug.LogWarning("PizzaListPrefab does not contain an Image component to assign the sprite to.");
        }
    }

    // Given the plate's ingredients, return the matching pizza sprite if any (per mapping provided)
    public Sprite GetMatchingPizzaSprite(Plate plate)
    {
        if (plate == null)
            return null;

        var ingredients = plate.GetIngredients(); // includes dough, sauce, cheese, toppings
        // require cheese to be present for any valid pizza
        bool hasCheese = ingredients.Contains("cheese");

        if (!hasCheese)
            return null;

        bool hasPineapple = ingredients.Contains("pineapple");
        bool hasMushroom = ingredients.Contains("mushroom");
        bool hasBasil = ingredients.Contains("basil");
        bool hasPepperoni = ingredients.Contains("pepperoni");

        // pizza1 - cheese no toppings
        if (!hasPineapple && !hasMushroom && !hasBasil && !hasPepperoni)
            return pizza1;

        // pizza2 - pepperoni
        if (hasPepperoni && !hasMushroom && !hasBasil && !hasPineapple)
            return pizza2;

        // pizza3 - mushroom
        if (hasMushroom && !hasBasil && !hasPineapple && !hasPepperoni)
            return pizza3;

        // pizza4 - mushroom and basil
        if (hasMushroom && hasBasil && !hasPineapple && !hasPepperoni)
            return pizza4;

        // pizza5 - mushroom basil pineapple
        if (hasMushroom && hasBasil && hasPineapple && !hasPepperoni)
            return pizza5;

        // pizza6 - pineapple
        if (hasPineapple && !hasMushroom && !hasBasil && !hasPepperoni)
            return pizza6;

        // pizza7 - basil and pineapple
        if (hasBasil && hasPineapple && !hasMushroom && !hasPepperoni)
            return pizza7;

        // pizza8 - pepperoni basil pineapple
        if (hasPepperoni && hasBasil && hasPineapple && !hasMushroom)
            return pizza8;

        return null;
    }

    // Removes the first child under HorizontalLayoutParent whose Image.sprite == spriteToRemove.
    // Returns true if removed.
    public bool RemovePizzaListEntryBySprite(Sprite spriteToRemove)
    {
        if (spriteToRemove == null)
            return false;

        if (HorizontalLayoutParent == null)
        {
            Debug.LogWarning("HorizontalLayoutParent is not assigned on GameController");
            return false;
        }

        foreach (Transform child in HorizontalLayoutParent.transform)
        {
            if (child == null)
                continue;

            Image img = child.GetComponent<Image>();
            if (img == null)
                img = child.GetComponentInChildren<Image>();

            if (img != null && img.sprite == spriteToRemove)
            {
                Destroy(child.gameObject);

                // Do NOT reset the customer/ui timers here so existing countdowns continue uninterrupted.
                // Make customer invisible now; the next timer cycle (ChangeCustomerSprite) will re-enable and set a new sprite.
                if (customerSpriteRenderer != null)
                    customerSpriteRenderer.enabled = false;

                // Make the UI pizza bubble invisible now; ChangeUIPizzaSprite() will re-enable on the next timer cycle.
                if (uiPizzaSpriteRenderer != null)
                    uiPizzaSpriteRenderer.enabled = false;
                if (uiPizzaImage != null)
                    uiPizzaImage.enabled = false;

                // Also hide the bubble container if assigned (so both customer and bubble are invisible)
                if (Bubble != null)
                    Bubble.SetActive(false);

                return true;
            }
        }

        return false;
    }

    // Register a plate as active. Returns true if successful, false if limit exceeded.
    // Allows 1 uncooked plate that is NOT cooking. Once a plate enters the oven, 
    // a new uncooked plate can be spawned.
    public bool RegisterPlate(Plate plate)
    {
        if (plate == null)
            return false;

        // Uncooked plates: only allow 1 at a time that is NOT in the oven
        if (!plate.IsCooked)
        {
            // If there's already an active uncooked plate AND it's not the same one
            if (activeUncookedPlate != null && activeUncookedPlate != plate)
            {
                // Only block if the existing plate is NOT currently cooking
                if (!isOvenCooking)
                {
                    Debug.Log("Cannot spawn uncooked plate: 1 uncooked plate already active (not cooking).");
                    return false;
                }
                // If oven IS cooking, allow this new plate to spawn (old one is in oven)
            }

            activeUncookedPlate = plate;
            return true;
        }

        // Cooked plates have no limit
        return true;
    }

    // Unregister a plate when it's destroyed
    public void UnregisterPlate(Plate plate)
    {
        if (plate == null)
            return;

        if (plate == activeUncookedPlate)
            activeUncookedPlate = null;
    }

    // Notify GameController when a plate changes cooking state
    public void OnPlateStateChanged(Plate plate)
    {
        if (plate == null)
            return;

        // Update tracking when plate state changes (uncooked -> cooked)
        if (plate.IsCooked && plate == activeUncookedPlate)
        {
            activeUncookedPlate = null;
        }
    }

    private void HandleIngredientSpawning(GameObject spawner, GameObject prefab)
    {
        if (spawner == null || prefab == null)
            return;

        BoxCollider2D spawnerCollider = spawner.GetComponent<BoxCollider2D>();
        BoxCollider2D cursorCollider = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();

        if (spawnerCollider == null || cursorCollider == null)
            return;

        // Spawn ingredient when cursor is over spawner and mouse is clicked
        if (Input.GetMouseButtonDown(0) && spawnerCollider.bounds.Intersects(cursorCollider.bounds) && currentlyDraggedObject == null)
        {
            Instantiate(prefab, spawner.transform.position, Quaternion.identity, parentObj);
        }
    }

    private void HandlePlateSpawning(GameObject spawner, GameObject prefab)
    {
        if (spawner == null || prefab == null)
            return;

        BoxCollider2D spawnerCollider = spawner.GetComponent<BoxCollider2D>();
        BoxCollider2D cursorCollider = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();

        if (spawnerCollider == null || cursorCollider == null)
            return;

        // Spawn plate when cursor is over spawner and mouse is clicked
        if (Input.GetMouseButtonDown(0) && spawnerCollider.bounds.Intersects(cursorCollider.bounds) && currentlyDraggedObject == null)
        {
            // Get cursor position and convert to world coordinates
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Instantiate(prefab, worldPosition, Quaternion.identity, parentObj);
        }
    }
}
