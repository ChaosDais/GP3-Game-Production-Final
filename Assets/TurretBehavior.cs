using JetBrains.Annotations;
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
    private float previousAngle;
    private float totalRotation;

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

    [Header("Damage Sphere")]
    [SerializeField] private GameObject damageSphere;
    [SerializeField] private float sphereRadius = 0.5f;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogWarning("no player Tag!.");
        }


        // Create damage sphere if it doesn't exist
        if (damageSphere == null)
        {
            damageSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            damageSphere.transform.localScale = Vector3.one * sphereRadius * 2;

            // makes the sphere invisible
            Destroy(damageSphere.GetComponent<MeshRenderer>());

            // this will make sure that the newly created trigger sphere has a true trigger.
            damageSphere.GetComponent<SphereCollider>().isTrigger = true;

            //made a new script for just the chip damage
            damageSphere.AddComponent<DamageTrigger>();
        }

        // Rotation for the head and 
        currentBodyRotation = bodyTransform.localEulerAngles.z;
        currentHeadRotation = headTransform.localEulerAngles.x;
        previousAngle = currentBodyRotation;
        totalRotation = currentBodyRotation;
    }

    public void OnSphereHitPlayer(Collider playerCollider)
    {
        // Apply damage every seconds depending on interval time
        if (Time.time - lastDamageTime >= damageInterval)
        {
            var damageable = playerCollider.GetComponent<DamageableCharacter>();
            if (damageable != null)
            {
                damageable.OnHit(laserDamage);
            }
            lastDamageTime = Time.time;
        }
    }
    public int uprotation = 60;

    private void Update()
    {


        if (playerTransform == null) return;

        // Get direction to player
        targetDirection = playerTransform.position - transform.position;

        // Calculate target rotation based on direction to player
        Vector3 rayDirection = (playerTransform.position - rayOriginTransform.position);
        float targetBodyRotation = Mathf.Atan2(rayDirection.y, rayDirection.x) * Mathf.Rad2Deg;

        // Ensure continuous rotation beyond 360 degrees
        float angleDelta = Mathf.DeltaAngle(previousAngle, targetBodyRotation);
        totalRotation = Mathf.Clamp(totalRotation + angleDelta, 0f, 720f);
        previousAngle = targetBodyRotation;

        // Use the total rotation to maintain continuous movement
        targetBodyRotation = totalRotation;
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


        Vector3 currentRotation = bodyTransform.localEulerAngles;
        currentRotation.z = currentBodyRotation + (-uprotation);
        bodyTransform.localEulerAngles = currentRotation;
        headTransform.localRotation = Quaternion.Euler(currentHeadRotation - uprotation, 0, 0);


        if (rayOriginTransform != null && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= laserDistance)
            {
              
               
                Vector3 rayStart = rayOriginTransform.position;
                Vector3 rayEnd = rayStart + rayDirection * rayDistance;


                bool hit = Physics.Raycast(rayStart, rayDirection, out raycastHitInfo, rayDistance, raycastLayerMask);
                if (hit)
                {
                    rayEnd = raycastHitInfo.point;


                }

                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = 2;

                    if (currentLaserEnd == Vector3.zero)
                        currentLaserEnd = rayStart;
                    currentLaserEnd = Vector3.Lerp(currentLaserEnd, rayEnd, Time.deltaTime * laserLerpSpeed);
                    lineRenderer.SetPosition(0, rayStart);
                    lineRenderer.SetPosition(1, currentLaserEnd);


                    if (damageSphere != null)
                    {
                        damageSphere.transform.position = currentLaserEnd;
                        damageSphere.SetActive(true);
                    }
                }
            }
            else
            {
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = false;
                }
                if (damageSphere != null)
                {
                    damageSphere.SetActive(false);
                }
                currentLaserEnd = Vector3.zero;
            }
        }
    }
}