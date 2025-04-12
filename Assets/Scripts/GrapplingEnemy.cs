using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingEnemy : MonoBehaviour
{
    public GameObject sightUI;
    public Transform enemyTargetPoint;

    private LineRenderer grapplingLine;
    private Vector3 hookPoint;
    private Transform pulledEnemy;
    private Vector3 enemyLocalPoint;
    public LayerMask enemyLayer;
    public Transform gunTip, playerCamera, player;

    private float maxGrappleDistance = 100f;
    private Rigidbody pulledEnemyRb;
    private Rigidbody playerRb;
    public float pullStopDistance = 0.5f;
    private Vector3 pullDirection;
    public float pullSpeed = 15f;
    private bool isGrappling = false;
    public float pullForce = 10f;
    private Vector3 currentHookPosition;

    // Input System
    private InputSystem_Actions inputActions;
    private bool isAiming = false;
    private bool firePressed = false;
    private bool cancelPressed = false;

    private void Awake()
    {
        grapplingLine = GetComponent<LineRenderer>();
        enemyLayer = LayerMask.GetMask("Enemy");
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Aim.performed += ctx => isAiming = true;
        inputActions.Player.Aim.canceled += ctx => isAiming = false;
        inputActions.Player.Interact.performed += ctx => firePressed = true;
        inputActions.Player.CancelGrapple.performed += ctx => cancelPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();

        grapplingLine.startWidth = 0.02f;
        grapplingLine.endWidth = 0.02f;

        if (enemyTargetPoint == null)
        {
            GameObject targetPoint = new GameObject("PullTargetPoint");
            enemyTargetPoint = targetPoint.transform;
            enemyTargetPoint.SetParent(player);
            enemyTargetPoint.localPosition = new Vector3(0, 0, 1f);
        }
    }

    void Update()
    {
        // Handle both mouse and controller input for aiming
        bool isMouseAiming = Input.GetMouseButton(1);
        bool isAimingInput = isMouseAiming || isAiming;

        if (isGrappling)
        {
            PullEnemyTowardsPlayer();

            // Only cancel if explicitly requested
            if (Input.GetKey(KeyCode.LeftShift) || cancelPressed)
            {
                StopEnemyGrapple();
                cancelPressed = false;
                return;
            }
        }

        // Only need aim input for targeting and starting the pull
        if (isAimingInput)
        {
            AimEnemyGrapple();
            ShowAimingUI();

            // Single press to start pull with either input method
            if ((Input.GetMouseButtonDown(0) && isMouseAiming) || (firePressed && !isMouseAiming))
            {
                if (!isGrappling)
                {
                    StartEnemyGrapple();
                }
                firePressed = false;
            }
        }
        else
        {
            HideAimingUI();
        }

        float distance = Vector3.Distance(playerCamera.position, hookPoint);
        float width = Mathf.Clamp(distance * 0.002f, 0.01f, 0.05f);
        grapplingLine.startWidth = width;
        grapplingLine.endWidth = width;
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
