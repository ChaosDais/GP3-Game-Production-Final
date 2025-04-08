using UnityEditor.Rendering;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    //Possible UI sight here not sure if this will work, video said it should tho
    public GameObject sightUI;

    // Line renderer and grappling stuff
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Transform grappledObject; // Reference to the object we're grappled to
    private Vector3 grappleLocalPoint; // Local position on the grappled object
    public LayerMask whatIsGrappleable; //Object needs this to have the grappling gun to stick to wherever it shoots
    public Transform gunTip, camera, player; // guntip will be the gun tip of grappling gun


    private float maxDistance = 100f; //how far grapple can shot
    private SpringJoint joint; //launch variable
    private Rigidbody rb;
    public float stopDistance = 1f; //how close player needs to be before stopping theg grappling function
    private Vector3 grappleDirection; // where player is flying to
    public float grappleSpeed = 10f; // how fast play is flying
    private bool isGrappling = false; //boolean for function should also add a delay to stop player from spamming grappling gun
    public float pullForce = 10f; // also affects the player reaching stopDistance
    private Vector3 currentGrapplePosition; // where player is shot grappling gun.

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = player.GetComponent<Rigidbody>();
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            AimGrapple();
            ShowAimingUI();
        }
        else
        {
            HideAimingUI();
        }

        if (Input.GetMouseButtonDown(0) && !IsGrappling())
        {
            StartGrapple();
            Debug.Log("Grappling");
        }

        if (IsGrappling())
        {
            PullPlayerTowardsGrapple();

            // LEFTSHIFTCANCEL
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log("Grapple cancelled by player");
                StopGrapple();
            }
        }

        else if (Input.GetMouseButtonUp(0) && IsGrappling())
        {
            StopGrapple();
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
    }

    void DrawRope()
    {
        if (!joint) return;
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
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
    }
}
