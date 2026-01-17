using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraFollowSpeed = 250f;
    [SerializeField] private float maxCameraDistance = 0.2f; // Maximum distance camera can move from initial position
    [SerializeField] private float deadzoneRadius = 8.5f; // Distance before camera starts moving

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
        float distanceToMouse = Vector2.Distance(mouseWorldPos, cameraBasePos);

        Vector2 targetCameraPos = cameraBasePos;

        // Only move camera if mouse is outside deadzone
        if (distanceToMouse > deadzoneRadius)
        {
            float effectiveDistance = Mathf.Min(distanceToMouse - deadzoneRadius, maxCameraDistance);
            Vector2 movement = directionToMouse * effectiveDistance;
            
            // Reduce vertical movement to half of horizontal
            movement.y *= 0.2f;
            movement.x *= 0.5f;

            targetCameraPos = cameraBasePos + movement;
        }

        // Smoothly move camera towards target
        Vector3 currentCameraPos = mainCamera.transform.position;
        currentCameraPos = Vector3.Lerp(currentCameraPos, new Vector3(targetCameraPos.x, targetCameraPos.y, currentCameraPos.z), Time.deltaTime * cameraFollowSpeed);
        mainCamera.transform.position = currentCameraPos;

        // Move cursor to mouse position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }
}
