using UnityEngine;

public class TurretBehavior : MonoBehaviour
{
    [Header("Turret Parts")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform headTransform;

    [Header("Tracking Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxHeadAngle = 60f;

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
    [SerializeField] private float laserLerpSpeed = 10f;
    private Vector3 currentLaserEnd;

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        // Find the player - you might want to assign this differently based on your setup
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
        }

        // Initialize current rotations
        currentBodyRotation = bodyTransform.localEulerAngles.z;
        currentHeadRotation = headTransform.localEulerAngles.x;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Get direction to player
        targetDirection = playerTransform.position - transform.position;

        // Vector Math to Follow player
        float targetBodyRotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        float targetHeadRotation = Mathf.Clamp(
            Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.right),
            -maxHeadAngle,
            maxHeadAngle
        );

        // Lerp Up and down
        currentBodyRotation = Mathf.LerpAngle(
            currentBodyRotation,
            targetBodyRotation,
            Time.deltaTime * rotationSpeed
        );

        // Lerp left and right
        currentHeadRotation = Mathf.LerpAngle(
            currentHeadRotation,
            targetHeadRotation,
            Time.deltaTime * rotationSpeed
        );

        bodyTransform.localRotation = Quaternion.Euler(0, 0, currentBodyRotation);
        headTransform.localRotation = Quaternion.Euler(currentHeadRotation, 0, 0);


        if (rayOriginTransform != null && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= laserDistance)
            {
                Vector3 rayDirection = (playerTransform.position - rayOriginTransform.position).normalized;
                Vector3 rayStart = rayOriginTransform.position;
                Vector3 rayEnd = rayStart + rayDirection * rayDistance;

                bool hit = Physics.Raycast(rayStart, rayDirection, out raycastHitInfo, rayDistance, raycastLayerMask);
                if (hit)
                {
                    rayEnd = raycastHitInfo.point;


                    if (raycastHitInfo.collider.CompareTag("Player"))
                    {
                        // Damage every couple seconds
                        if (Time.time - lastDamageTime >= damageInterval)
                        {
                            var damageable = raycastHitInfo.collider.GetComponent<DamageableCharacter>();
                            if (damageable != null)
                            {
                                damageable.Health -= laserDamage;

                                lastDamageTime = Time.time;
                            }
                        }
                    }

                    if (lineRenderer != null)
                    {
                        lineRenderer.enabled = true;
                        lineRenderer.positionCount = 2;
                        //Lerp Method
                        if (currentLaserEnd == Vector3.zero)
                            currentLaserEnd = rayStart;
                        currentLaserEnd = Vector3.Lerp(currentLaserEnd, rayEnd, Time.deltaTime * laserLerpSpeed);
                        lineRenderer.SetPosition(0, rayStart);
                        lineRenderer.SetPosition(1, currentLaserEnd);
                    }
                }
                else
                {
                    // Disable the LineRenderer if player is out of range
                    if (lineRenderer != null)
                    {
                        lineRenderer.enabled = false;
                    }
                    currentLaserEnd = Vector3.zero; // Reset for next activation
                }
            }
        }
    }
}