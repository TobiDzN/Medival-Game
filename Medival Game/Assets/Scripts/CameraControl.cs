using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target; // The character the camera follows
    public float maxDistance = 15.0f; // Maximum zoom distance
    public float minDistance = 3.0f; // Minimum zoom distance
    public float zoomSpeed = 2.0f; // Speed of zooming
    public float sensitivity = 2.0f; // Mouse sensitivity

    public float minXRotation = 10f; // Minimum X rotation (prevents looking too high)
    public float maxXRotation = 80f; // Maximum X rotation (prevents looking too low)

    private float distance; // Current zoom distance
    private float currentX = 45f; // Vertical rotation (X-axis)
    private float currentY = 0f;  // Horizontal rotation (Y-axis)

    void Start()
    {
        // Start at max zoom out
        distance = maxDistance;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input for rotation
        currentY += Input.GetAxis("Mouse X") * sensitivity;
        currentX -= Input.GetAxis("Mouse Y") * sensitivity; // Inverted Y-axis movement

        // Clamp X rotation to prevent flipping
        currentX = Mathf.Clamp(currentX, minXRotation, maxXRotation);

        // Zoom in/out with mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        // Calculate new camera position based on X and Y rotation
        Quaternion rotation = Quaternion.Euler(currentX, currentY, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + offset;

        // Look at the target
        transform.LookAt(target.position);
    }
}
