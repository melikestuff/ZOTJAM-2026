using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool isDoughPlaced = false;
    public bool isSauceAdded = false;
    public bool isCheeseAdded = false;
    public GameObject ovenProgress1;
    
    public static MonoBehaviour currentlyDraggedObject = null;
    
    public bool isOvenCooking = false;
    public float ovenTimer = 0f;
    public float ovenCookTime = 5f; // Time in seconds for cooking
    
    void Start()
    {
        
    }

    void Update()
    {
        //Debug.Log("Dough: " + isDoughPlaced + " Sauce: " + isSauceAdded + " Cheese: " + isCheeseAdded);
        
        // Update oven timer
        if (isOvenCooking)
        {
            ovenTimer -= Time.deltaTime;
            
            // Update progress bar
            if (ovenProgress1 != null)
            {
                float progress = (1f - (ovenTimer / ovenCookTime)) * 2.6f;
                ovenProgress1.transform.localScale = new Vector3(progress, 1f, 1f);
            }
            
            // Check if cooking is done
            if (ovenTimer <= 0f)
            {
                isOvenCooking = false;
                ovenTimer = 0f;
                
                // Update progress bar to full
                if (ovenProgress1 != null)
                {
                    ovenProgress1.transform.localScale = new Vector3(2.6f, 1f, 1f);
                }
            }
        }
    }
}
