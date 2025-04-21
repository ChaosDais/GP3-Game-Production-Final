using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [Header("Turret Parts")]
    [SerializeField] private Transform bodyTransform;    // Z-axis rotation (left/right)
    [SerializeField] private Transform headTransform;    // X-axis rotation (up/down)

    [Header("Tracking Settings")]
    [SerializeField] private float rotationSpeed = 5f;   // Speed of rotationa
    [SerializeField] private float maxHeadAngle = 60f;   // Maximum up/down angle

    private Transform playerTransform;
    private Vector3 targetDirection;
    private float currentBodyRotation;
    private float currentHeadRotation;

    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOriginTransform;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private LayerMask raycastLayerMask = ~0;
    private RaycastHit raycastHitInfo;
    public float laserDistance = 30f;

    [Header("Laser Damage")]
    [SerializeField] private int laserDamage = 10;
    [SerializeField] private float damageInterval = 3f;
    private float lastDamageTime = -Mathf.Infinity;

    [Header("Laser Visuals")]
    [SerializeField] private float laserLerpSpeed = 5f;
    private Vector3 currentLaserEnd;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogWarning("no player Tag!.");
        }

        // Rotation for the head and 
        currentBodyRotation = bodyTransform.localEulerAngles.z;
        currentHeadRotation = headTransform.localEulerAngles.x;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Get direction to player
        targetDirection = playerTransform.position - transform.position;

        // Calculate target rotations
        float targetBodyRotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float targetHeadRotation = Mathf.Clamp(
            Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.right),
            -maxHeadAngle,
            maxHeadAngle
        );

        // Smoothly rotate body (Z-axis)
        currentBodyRotation = Mathf.LerpAngle(
            currentBodyRotation,
            targetBodyRotation,
            Time.deltaTime * rotationSpeed
        );

        // Smoothly rotate head (X-axis)
        currentHeadRotation = Mathf.LerpAngle(
            currentHeadRotation,
            targetHeadRotation,
            Time.deltaTime * rotationSpeed
        );

        // Apply rotations
        bodyTransform.localRotation = Quaternion.Euler(0, 0, currentBodyRotation);
        headTransform.localRotation = Quaternion.Euler(currentHeadRotation, 0, 0);

        // Raycast toward player from rayOriginTransform only if within 10 units
        if (rayOriginTransform != null && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= laserDistance)
            {
                Vector3 rayDirection = (playerTransform.position - rayOriginTransform.position).normalized;
                Vector3 rayStart = rayOriginTransform.position;
                Vector3 rayEnd = rayStart + rayDirection * rayDistance;

                // Perform the raycast
                bool hit = Physics.Raycast(rayStart, rayDirection, out raycastHitInfo, rayDistance, raycastLayerMask);
                if (hit)
                {
                    rayEnd = raycastHitInfo.point;

                    // This checks if player is hit
                    if (raycastHitInfo.collider.CompareTag("Player"))
                    {
                        // Apply damage every seconds depending on interval time
                        if (Time.time - lastDamageTime >= damageInterval)
                        {
                            var damageable = raycastHitInfo.collider.GetComponent<DamageableCharacter>();
                            if (damageable != null)
                            {
                                damageable.OnHit(laserDamage);
                            }
                            lastDamageTime = Time.time;
                        }
                    }
                }

                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = 2;
                    // Smoothly lerp the end of the laser toward the target
                    if (currentLaserEnd == Vector3.zero)
                        currentLaserEnd = rayStart; // Initialize on first use
                    currentLaserEnd = Vector3.Lerp(currentLaserEnd, rayEnd, Time.deltaTime * laserLerpSpeed);
                    lineRenderer.SetPosition(0, rayStart);
                    lineRenderer.SetPosition(1, currentLaserEnd);
                }
            }
            else
            {
                // turn off the line renderer if player oto far
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = false;
                }
                currentLaserEnd = Vector3.zero; // Reset laser end so that it doesnt stay there on reactivation
            }
        }
    }
}