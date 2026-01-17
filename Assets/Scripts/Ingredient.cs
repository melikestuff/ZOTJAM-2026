using UnityEditor;
using UnityEngine;

public class Ingredient : MonoBehaviour

{
    [SerializeField] private BoxCollider2D cursor;
    [SerializeField] private BoxCollider2D plate;
    [SerializeField] private bool isPlaced = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the cursor object tagged as "Cursor" and get its BoxCollider2D component
        
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<BoxCollider2D>();
        plate = GameObject.FindGameObjectWithTag("Plate").GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // if mouse button is down, move the ingredient to the mouse position


        if (Input.GetMouseButton(0))
        {
            // Check if the ingredient is overlapping with the cursor collider
            if (cursor.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                //if is not placed
                if (!isPlaced)
                {

                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = 10; // Set this to be the distance from the camera
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
                }
            }


        }

        //if mouse is released and is overlapping with plate, set isplaced to true and snap to plate position
        if (Input.GetMouseButtonUp(0))
        {
            if (plate.bounds.Intersects(GetComponent<Collider2D>().bounds))
            {
                isPlaced = true;
                transform.position = new Vector3(plate.transform.position.x, plate.transform.position.y, transform.position.z);
            }
        }

    }
}
