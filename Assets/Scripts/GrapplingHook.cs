using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
public class GrapplingHook : MonoBehaviour
{
    //Possible UI sight here not sure if this will work, video said it should tho
    public GameObject sightUI;

    // Grappling Hook object and animation
    private GameObject grappleHookObject;
    private Animator grappleAnimator;
    private bool isHookAnimating = false;

    // Line renderer and grappling stuff
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Transform grappledObject; // Reference to the object we're grappled to
    private Vector3 grappleLocalPoint; // Local position on the grappled object
    public LayerMask whatIsGrappleable; //Object needs this to have the grappling gun to stick to wherever it shoots
    public Transform gunTip, camera, player; // guntip will be the gun tip of grappling gun


    private float maxDistance = 350f; //how far grapple can shot
    private SpringJoint joint; //launch variable
    private Rigidbody rb;
    public float stopDistance = 1f; //how close player needs to be before stopping theg grappling function
    private Vector3 grappleDirection; // where player is flying to
    public float grappleSpeed = 10f; // how fast play is flying
    private bool isGrappling = false; //boolean for function should also add a delay to stop player from spamming grappling gun
    public float pullForce = 10f; // also affects the player reaching stopDistance
    private Vector3 currentGrapplePosition; // where player is shot grappling gun.

    // Input System
    private InputSystem_Actions inputActions;
    private bool isAiming = false;
    private bool firePressed = false;
    private bool cancelPressed = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();

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
       
        rb = player.GetComponent<Rigidbody>();
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;

        // Find and initially hide the grappling hook object
        grappleHookObject = GameObject.FindGameObjectWithTag("Graphook");
        if (grappleHookObject != null)
        {
            grappleAnimator = grappleHookObject.GetComponent<Animator>();
            grappleHookObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No object with tag 'Graphook' found!");
        }
    }


    void Update()
    {
        if (Input.GetMouseButton(1) || isAiming)
        {
            AimGrapple();
            ShowAimingUI();

            // Start grapple when either using mouse controls or input system
            if ((Input.GetMouseButtonDown(0)) || (isAiming && firePressed))
            {
                if (!IsGrappling())
                {
                    StartGrapple();
                    Debug.Log("Grappling");
                }
                firePressed = false;
            }
        }
        else
        {
            HideAimingUI();
        }

        if (IsGrappling())
        {
            PullPlayerTowardsGrapple();

            // Cancel grapple with either left shift or cancel button
            if (Input.GetKey(KeyCode.LeftShift) || cancelPressed)
            {
                Debug.Log("Grapple cancelled by player");
                StopGrapple();
                cancelPressed = false;
            }
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
        DrawRope();
    }

    void AimGrapple()
    {
        // Show and animate grappling hook if not already doing so
        if (!isHookAnimating && grappleHookObject != null)
        {
            grappleHookObject.SetActive(true);
            grappleHookObject.transform.position = gunTip.position;
            grappleAnimator.SetTrigger("Idle"); // Make sure you have an "Idle" animation parameter
            isHookAnimating = true;
        }

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            currentGrapplePosition = grapplePoint;
        }
        else
        {
            grapplePoint = camera.position + camera.forward * maxDistance;
        }
    }

    void StartGrapple()
    {
        Debug.Log("GRAPPING FR FR");
        isGrappling = true;

        // Stop idle animation and start grapple animation
        if (grappleHookObject != null && grappleAnimator != null)
        {
            grappleAnimator.SetTrigger("Shoot"); // Make sure you have a "Shoot" animation parameter
        }

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grappledObject = hit.transform;
            grappleLocalPoint = grappledObject.InverseTransformPoint(hit.point);
            grapplePoint = hit.point;
            grappleDirection = (grapplePoint - player.position).normalized;

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 2f;
            joint.minDistance = distanceFromPoint * .2f;

            joint.spring = 100f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
        isGrappling = false;
        lr.enabled = false;
        grappledObject = null;

        // Hide grappling hook and reset animation state
        if (grappleHookObject != null)
        {
            grappleHookObject.SetActive(false);
            isHookAnimating = false;
        }
    }

    void DrawRope()
    {
        if (!joint) return;
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);

       
        if (grappleHookObject != null && isGrappling)
        {
            grappleHookObject.transform.position = currentGrapplePosition;
            
            grappleHookObject.transform.LookAt(grapplePoint);
        }
    }

    void PullPlayerTowardsGrapple()
    {
        if (joint == null) return;

        if (grappledObject != null)
        {
            // Update grapple point position for moving objects
            grapplePoint = grappledObject.TransformPoint(grappleLocalPoint);
            joint.connectedAnchor = grapplePoint;

            // Continuously update direction vector to follow moving target
            grappleDirection = (grapplePoint - player.position).normalized;
        }

        float distanceToTarget = Vector3.Distance(player.position, grapplePoint);

        // Calculate speed multiplier - maintain full speed until very close to target
        float speedMultiplier = 1f;
        if (distanceToTarget < stopDistance * 2f)
        {
            speedMultiplier = Mathf.Lerp(0.5f, 1f, distanceToTarget / (stopDistance * 2f));
        }

        // Apply velocity directly towards target
        Vector3 targetVelocity = grappleDirection * (grappleSpeed * speedMultiplier);
        rb.velocity = targetVelocity;

        currentGrapplePosition = grapplePoint;

        // Only stop grappling if we're close enough AND not attached to a moving platform
        if (distanceToTarget < stopDistance && grappledObject == null)
        {
            StopGrapple();
            Debug.Log("Grapple Stopped - Distance: " + distanceToTarget);
        }
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
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

        // Hide grappling hook when not aiming
        if (grappleHookObject != null && !isGrappling)
        {
            grappleHookObject.SetActive(false);
            isHookAnimating = false;
        }
    }
}
