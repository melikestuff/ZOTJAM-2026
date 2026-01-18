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

    public static MonoBehaviour currentlyDraggedObject = null;
    
    public bool isOvenCooking = false;
    public float plateTimer = 0f;
    public float plateCookTime = 5f; // Time in seconds for cooking
    
    private BoxCollider2D plateCollider = null;
    
    void Start()
    {
        plateSpawner = GameObject.FindGameObjectWithTag("PlateSpawner");
        sauceSpawner = GameObject.FindGameObjectWithTag("SauceSpawner");
        cheeseSpawner = GameObject.FindGameObjectWithTag("CheeseSpawner");
        doughSpawner = GameObject.FindGameObjectWithTag("DoughSpawner");
        
        plateCollider = GameObject.FindGameObjectWithTag("Plate").GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        HandleIngredientSpawning(doughSpawner, doughPrefab);
        HandleIngredientSpawning(sauceSpawner, saucePrefab);
        HandleIngredientSpawning(cheeseSpawner, cheesePrefab);
        HandlePlateSpawning(plateSpawner, platePrefab);
        
        // Update plate timer
        if (isOvenCooking)
        {
            plateTimer -= Time.deltaTime;
            
            // Update progress bar
            if (plateProgress1 != null)
            {
                float progress = (1f - (plateTimer / plateCookTime)) * 2.6f;
                plateProgress1.transform.localScale = new Vector3(progress, 1f, 1f);
            }
            
            // Check if cooking is done
            if (plateTimer <= 0f)
            {
                isOvenCooking = false;
                plateTimer = 0f;
                
                // Update progress bar to full
                if (plateProgress1 != null)
                {
                    plateProgress1.transform.localScale = new Vector3(2.6f, 1f, 1f);
                }
            }
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
