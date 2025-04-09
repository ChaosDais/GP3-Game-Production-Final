using UnityEditor.Rendering;
using UnityEngine;

public class GrapplingPulls : MonoBehaviour
{
    public GameObject sightUI;
    public Transform pullTargetPoint;

    private LineRenderer pullLine;
    private Vector3 pullPoint;
    private Transform pulledObject;
    private Vector3 pullLocalPoint;
    public LayerMask whatIsPullable;
    public Transform gunTip, camera, player;

    private float maxPullDistance = 100f;
    private Rigidbody pulledObjectRb;
    private Rigidbody playerRb;
    public float pullStopDistance = 0.5f;
    private Vector3 pullDirection;
    public float pullSpeed = 15f;
    private bool isPulling = false;
    private bool isHolding = false;
    public float pullForce = 10f;
    private Vector3 currentPullPosition;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
        pullLine.startWidth = 0.02f;
        pullLine.endWidth = 0.02f;


        if (pullTargetPoint == null)
        {
            GameObject targetPoint = new GameObject("PullTargetPoint");
            pullTargetPoint = targetPoint.transform;
            pullTargetPoint.SetParent(player);
            pullTargetPoint.localPosition = new Vector3(0, 0, 1f);
        }
    }

    void Awake()
    {
        pullLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (isHolding)
        {
            if (pulledObjectRb != null)
            {
                pulledObjectRb.velocity = Vector3.zero;
                pulledObjectRb.position = pullTargetPoint.position;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                ReleaseObject();
            }
            return;
        }

        if (isPulling)
        {
            PullObjectTowardsPlayer();

            if (Input.GetKey(KeyCode.LeftShift) || !Input.GetMouseButton(1))
            {
                StopPull();
                return;
            }
        }

        if (Input.GetMouseButton(1))
        {
            AimPull();
            ShowAimingUI();
            if (Input.GetMouseButtonDown(0) && !isPulling && !isHolding)
            {
                StartPull();
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
        DrawPullLine();
        float distance = Vector3.Distance(camera.position, pullLocalPoint);
        float width = Mathf.Clamp(distance * 0.002f, 0.01f, 0.05f); // tweak multiplier & clamp as needed
        pullLine.startWidth = width;
        pullLine.endWidth = width;
    }

    void AimPull()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxPullDistance, whatIsPullable))
        {
            pullPoint = hit.point;
            currentPullPosition = pullPoint;
        }
        else
        {
            pullPoint = camera.position + camera.forward * maxPullDistance;
        }
    }

    void StartPull()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxPullDistance, whatIsPullable))
        {
            isPulling = true;
            pulledObject = hit.transform;
            pullLocalPoint = pulledObject.InverseTransformPoint(hit.point);
            pullPoint = hit.point;

            pulledObjectRb = pulledObject.GetComponent<Rigidbody>();
            if (pulledObjectRb == null)
            {
                Debug.LogWarning("Pullable object must have a Rigidbody component!");
                StopPull();
                return;
            }

            pullLine.positionCount = 2;
            pullLine.enabled = true;
        }
    }

    void PullObjectTowardsPlayer()
    {
        if (pulledObject == null || pulledObjectRb == null) return;

        pullPoint = pulledObject.TransformPoint(pullLocalPoint);
        pullDirection = (pullTargetPoint.position - pulledObjectRb.position).normalized;
        float distanceToTarget = Vector3.Distance(pulledObjectRb.position, pullTargetPoint.position);

        float speedMultiplier = 1f;
        if (distanceToTarget < pullStopDistance * 2f)
        {
            speedMultiplier = Mathf.Lerp(0.5f, 1f, distanceToTarget / (pullStopDistance * 2f));
        }

        Vector3 targetVelocity = pullDirection * (pullSpeed * speedMultiplier);
        pulledObjectRb.velocity = targetVelocity;

        currentPullPosition = pullPoint;

        if (distanceToTarget < pullStopDistance)
        {
            StartHolding();
        }
    }

    void StartHolding()
    {
        isPulling = false;
        isHolding = true;

        if (pulledObjectRb != null)
        {
            pulledObjectRb.velocity = Vector3.zero;
            pulledObjectRb.useGravity = false;
            pulledObjectRb.position = pullTargetPoint.position;
        }
    }

    void ReleaseObject()
    {
        if (pulledObjectRb != null)
        {
            pulledObjectRb.useGravity = true;
        }

        isHolding = false;
        StopPull();
    }

    void StopPull()
    {
        pullLine.positionCount = 0;
        isPulling = false;
        isHolding = false;
        pullLine.enabled = false;
        pulledObject = null;
        pulledObjectRb = null;
    }

    void DrawPullLine()
    {
        if (!isPulling && !isHolding) return;

        // Make sure pullPoint tracks the object as it moves
        if (pulledObject != null)
        {
            pullPoint = pulledObject.TransformPoint(pullLocalPoint);
        }

        pullLine.SetPosition(0, gunTip.position);
        pullLine.SetPosition(1, pullPoint);
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
