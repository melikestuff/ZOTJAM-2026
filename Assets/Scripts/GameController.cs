using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool isDoughPlaced = false;
    public bool isSauceAdded = false;
    public bool isCheeseAdded = false;
    public GameObject plateProgress1;
    [SerializeField] private GameObject plateSpawner;
    [SerializeField] private GameObject sauceSpawner;
    [SerializeField] private GameObject cheeseSpawner;
    [SerializeField] private GameObject doughSpawner;
    [SerializeField] private GameObject platePrefab;
    [SerializeField] private GameObject saucePrefab;
    [SerializeField] private GameObject cheesePrefab;
    [SerializeField] private GameObject doughPrefab;
    [SerializeField] private GameObject customerPrefab;
    public Transform customerSpawnPoint;

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

    public static MonoBehaviour currentlyDraggedObject = null;
    
    public bool isOvenCooking = false;
    public float plateTimer = 0f;
    public float plateCookTime = 5f; // Time in seconds for cooking
    
    private BoxCollider2D plateCollider = null;
    private GameObject customer = null;
    private SpriteRenderer customerSpriteRenderer = null;
    private float spriteChangeTimer = 0f;
    private float spriteChangeInterval = 5f; // Change sprite every 5 seconds
    
    void Start()
    {
        plateSpawner = GameObject.FindGameObjectWithTag("PlateSpawner");
        sauceSpawner = GameObject.FindGameObjectWithTag("SauceSpawner");
        cheeseSpawner = GameObject.FindGameObjectWithTag("CheeseSpawner");
        doughSpawner = GameObject.FindGameObjectWithTag("DoughSpawner");
        
        //plateCollider = GameObject.FindGameObjectWithTag("Plate").GetComponent<BoxCollider2D>();

        // Spawn customer prefab with random sprite gen1-10 at customer spawner position
        if (customerPrefab != null && customerSpawnPoint != null)
        {
            customer = Instantiate(customerPrefab, customerSpawnPoint.position, Quaternion.identity);
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
    }

    void Update()
    {
        HandleIngredientSpawning(doughSpawner, doughPrefab);
        HandleIngredientSpawning(sauceSpawner, saucePrefab);
        HandleIngredientSpawning(cheeseSpawner, cheesePrefab);
        HandlePlateSpawning(plateSpawner, platePrefab);
        
        // Update customer sprite every 5 seconds
        if (customerSpriteRenderer != null)
        {
            spriteChangeTimer -= Time.deltaTime;
            if (spriteChangeTimer <= 0f)
            {
                ChangeCustomerSprite();
                spriteChangeTimer = spriteChangeInterval;
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
        
        Sprite[] customerSprites = { gen1, gen2, gen3, gen4, gen5, gen6, gen7, gen8, gen9, gen10 };
        int randomIndex = Random.Range(0, customerSprites.Length);
        customerSpriteRenderer.sprite = customerSprites[randomIndex];
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
            Instantiate(prefab, spawner.transform.position, Quaternion.identity);
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
            Instantiate(prefab, spawner.transform.position, Quaternion.identity);
        }
    }
}
