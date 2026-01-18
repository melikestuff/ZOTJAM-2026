using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraFollowSpeed = 25f;
    [SerializeField] private float maxCameraDistance = 0.2f; // Maximum distance camera can move from initial position
    [SerializeField] private float maxCameraDistanceVertical = 1f; // Maximum vertical distance camera can move
    [SerializeField] private float maxCameraDistanceVerticalDown = 1f; // Maximum downward distance camera can move
    [SerializeField] private float deadzoneRadius = 8.5f; // Distance before camera starts moving
    public bool isDragging = false;

    private Vector3 initialCameraPosition;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        initialCameraPosition = mainCamera.transform.position;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Calculate target camera position (follows mouse with deadzone)
        Vector2 mouseWorldPos = new Vector2(worldPosition.x, worldPosition.y);
        Vector2 cameraBasePos = new Vector2(initialCameraPosition.x, initialCameraPosition.y);
        Vector2 directionToMouse = (mouseWorldPos - cameraBasePos).normalized;
        
        // Calculate distances with different deadzones for horizontal and vertical
        float horizontalDistance = Mathf.Abs(mouseWorldPos.x - cameraBasePos.x);
        float verticalDistance = Mathf.Abs(mouseWorldPos.y - cameraBasePos.y);
        float horizontalDeadzone = deadzoneRadius;
        float verticalDeadzone = deadzoneRadius * 0.5f;

        Vector2 targetCameraPos = cameraBasePos;

        // Only move camera if mouse is outside deadzone
        if (horizontalDistance > horizontalDeadzone || verticalDistance > verticalDeadzone)
        {
            Vector2 movement = Vector2.zero;
            
            if (horizontalDistance > horizontalDeadzone)
            {
                movement.x = Mathf.Sign(mouseWorldPos.x - cameraBasePos.x) * maxCameraDistance * 0.5f;
            }
            if (verticalDistance > verticalDeadzone)
            {
                // Use different max distances for up and down movement
                float maxVerticalDist = (mouseWorldPos.y > cameraBasePos.y) ? maxCameraDistanceVertical : maxCameraDistanceVerticalDown;
                movement.y = Mathf.Sign(mouseWorldPos.y - cameraBasePos.y) * maxVerticalDist;
            }

            targetCameraPos = cameraBasePos + movement;
        }

        // Smoothly move camera towards target
        Vector3 currentCameraPos = mainCamera.transform.position;
        currentCameraPos = Vector3.Lerp(currentCameraPos, new Vector3(targetCameraPos.x, targetCameraPos.y, currentCameraPos.z), cameraFollowSpeed * Time.deltaTime);
        mainCamera.transform.position = currentCameraPos;

        // Move cursor to mouse position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }
}
