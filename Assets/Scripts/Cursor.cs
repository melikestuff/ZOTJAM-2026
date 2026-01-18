using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraFollowSpeed = 25f;
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

        // Only vertical camera movement — horizontal movement removed
        Vector2 mouseWorldPos = new Vector2(worldPosition.x, worldPosition.y);
        Vector2 cameraBasePos = new Vector2(initialCameraPosition.x, initialCameraPosition.y);

        float verticalDistance = Mathf.Abs(mouseWorldPos.y - cameraBasePos.y);
        float verticalDeadzone = deadzoneRadius * 0.5f;

        Vector2 targetCameraPos = cameraBasePos;
        if (verticalDistance > verticalDeadzone)
        {
            float maxVerticalDist = (mouseWorldPos.y > cameraBasePos.y) ? maxCameraDistanceVertical : maxCameraDistanceVerticalDown;
            float movementY = Mathf.Sign(mouseWorldPos.y - cameraBasePos.y) * maxVerticalDist;
            targetCameraPos = new Vector2(cameraBasePos.x, cameraBasePos.y + movementY);
        }

        // Smoothly move camera towards target (x locked to initial x)
        Vector3 currentCameraPos = mainCamera.transform.position;
        currentCameraPos = Vector3.Lerp(currentCameraPos, new Vector3(targetCameraPos.x, targetCameraPos.y, currentCameraPos.z), cameraFollowSpeed * Time.deltaTime);
        mainCamera.transform.position = currentCameraPos;

        // Move cursor to mouse position
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }
}
