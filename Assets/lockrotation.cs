using UnityEngine;

public class lockrotation : MonoBehaviour
{
    public Transform player;        // Reference to the player
    public Transform cameraTransform; // Reference to the camera
    public float distanceFromPlayer = 2f; // How far from player
    public float heightOffset = 1f;      // How high above player
    public float forwardOffset = 1f;     // How far forward from player

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (player == null)
            Debug.LogError("Please assign the player Transform!");
    }

    void LateUpdate()
    {
        if (player == null || cameraTransform == null) return;

        // Calculate the desired position
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Keep it level with the ground
        cameraForward.Normalize();

        // Position the object relative to player
        Vector3 targetPosition = player.position;
        targetPosition.y += heightOffset; // Maintain height offset
        targetPosition += cameraForward * forwardOffset; // Keep forward offset

        // Update position while maintaining Z distance
        transform.position = targetPosition;

        // Make it face the camera
        transform.rotation = Quaternion.LookRotation(-cameraForward);
    }

    // Optional: Add methods to adjust the offsets at runtime
    public void SetHeight(float newHeight)
    {
        heightOffset = newHeight;
    }

    public void SetForwardOffset(float newOffset)
    {
        forwardOffset = newOffset;
    }
}