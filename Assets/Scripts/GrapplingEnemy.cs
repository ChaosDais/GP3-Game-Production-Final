using UnityEditor.Rendering;
using UnityEngine;

public class GrapplingEnemy : MonoBehaviour
{
    public GameObject sightUI;
    public Transform enemyTargetPoint; // Reference to where the enemy should be pulled to

    private LineRenderer grapplingLine;
    private Vector3 hookPoint;
    private Transform pulledEnemy;
    private Vector3 enemyLocalPoint;
    public LayerMask enemyLayer;
    public Transform gunTip, playerCamera, player;

    private float maxGrappleDistance = 100f;
    private Rigidbody pulledEnemyRb;
    private Rigidbody playerRb;
    public float pullStopDistance = 0.5f; // Distance at which to stop pulling the enemy
    private Vector3 pullDirection;
    public float pullSpeed = 15f;
    private bool isGrappling = false;
    public float pullForce = 10f;
    private Vector3 currentHookPosition;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerRb = player.GetComponent<Rigidbody>();

        // If enemyTargetPoint is not assigned, create it
        if (enemyTargetPoint == null)
        {
            GameObject targetPoint = new GameObject("PullTargetPoint");
            enemyTargetPoint = targetPoint.transform;
            enemyTargetPoint.SetParent(player);
            enemyTargetPoint.localPosition = new Vector3(0, 0, 1f); // 1 unit in front of the player
        }
    }

    void Awake()
    {
        grapplingLine = GetComponent<LineRenderer>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        if (isGrappling)
        {
            PullEnemyTowardsPlayer();

            if (Input.GetKey(KeyCode.LeftShift) || !Input.GetMouseButton(1))
            {
                StopEnemyGrapple();
                return;
            }
        }

        if (Input.GetMouseButton(1))
        {
            AimEnemyGrapple();
            ShowAimingUI();
            if (Input.GetMouseButtonDown(0) && !isGrappling)
            {
                StartEnemyGrapple();
            }
        }
        else
        {
            HideAimingUI();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    void LateUpdate()
    {
        DrawEnemyGrappleLine();
    }

    void AimEnemyGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, enemyLayer))
        {
            hookPoint = hit.point;
            currentHookPosition = hookPoint;
        }
        else
        {
            hookPoint = playerCamera.position + playerCamera.forward * maxGrappleDistance;
        }
    }

    void StartEnemyGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxGrappleDistance, enemyLayer))
        {
            isGrappling = true;
            pulledEnemy = hit.transform;
            enemyLocalPoint = pulledEnemy.InverseTransformPoint(hit.point);
            hookPoint = hit.point;
            
            pulledEnemyRb = pulledEnemy.GetComponent<Rigidbody>();
            if (pulledEnemyRb == null)
            {
                Debug.LogWarning("Enemy must have a Rigidbody component!");
                StopEnemyGrapple();
                return;
            }

            grapplingLine.positionCount = 2;
            grapplingLine.enabled = true;
        }
    }

    void PullEnemyTowardsPlayer()
    {
        if (pulledEnemy == null || pulledEnemyRb == null) return;

        hookPoint = pulledEnemy.TransformPoint(enemyLocalPoint);
        pullDirection = (enemyTargetPoint.position - pulledEnemyRb.position).normalized;
        float distanceToTarget = Vector3.Distance(pulledEnemyRb.position, enemyTargetPoint.position);

        float speedMultiplier = 1f;
        if (distanceToTarget < pullStopDistance * 2f)
        {
            speedMultiplier = Mathf.Lerp(0.5f, 1f, distanceToTarget / (pullStopDistance * 2f));
        }

        Vector3 targetVelocity = pullDirection * (pullSpeed * speedMultiplier);
        pulledEnemyRb.velocity = targetVelocity;

        currentHookPosition = hookPoint;

        if (distanceToTarget < pullStopDistance)
        {
            StopEnemyGrapple();
        }
    }

    void StopEnemyGrapple()
    {
        grapplingLine.positionCount = 0;
        isGrappling = false;
        grapplingLine.enabled = false;
        pulledEnemy = null;
        pulledEnemyRb = null;
    }

    void DrawEnemyGrappleLine()
    {
        if (!isGrappling) return;
        currentHookPosition = Vector3.Lerp(currentHookPosition, hookPoint, Time.deltaTime * 12f);
        grapplingLine.SetPosition(0, gunTip.position);
        grapplingLine.SetPosition(1, currentHookPosition);
    }

    private void ShowAimingUI()
    {
        if (sightUI != null)
        {
            sightUI.SetActive(true);
        }
    }

    private void HideAimingUI()
    {
        if (sightUI != null)
        {
            sightUI.SetActive(false);
        }

    }


}
