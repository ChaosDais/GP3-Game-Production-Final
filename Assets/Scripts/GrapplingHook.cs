using UnityEditor.Rendering;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject sightUI;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private Rigidbody rb;
    public float stopDistance = 1f;
    private Vector3 grappleDirection;
    public float grappleSpeed = 10f;
    private bool isGrappling = false;
    public float pullForce = 10f;
    private Vector3 currentGrapplePosition;

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
            grapplePoint = hit.point;
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

     
        Vector3 directionToGrapple = (grapplePoint - player.position).normalized;

      
        Vector3 velocity = directionToGrapple * grappleSpeed;

      
        rb.velocity = velocity;

       
        currentGrapplePosition = player.position + directionToGrapple * Vector3.Distance(player.position, grapplePoint);

        Debug.Log("Lerping towards Grapple Point: " + player.position);

        if (Vector3.Distance(player.position, grapplePoint) < stopDistance)
        {
            StopGrapple();
            Debug.Log("Grapple Stopped");
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
