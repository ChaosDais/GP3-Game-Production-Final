using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Tooltip("This interactable's sphere collider (Must be set to Trigger; there must only be one sphere collider)")]
    public SphereCollider sphereTriggerCollider;
    [Tooltip("Actual interactable model, used for distance calculations.")]
    public Transform interactable;
    [Tooltip("Events to run when interactings.")]
    public UnityEvent interactEvent;
    [Tooltip("Event that happens when you exit interactables.")]
    public UnityEvent leaveEvent;
    [Tooltip("Event that happens when an interactable is activateds.")]
    public UnityEvent activateEvent;
    [Tooltip("Canvas that says 'you can interact with this'")]
    public GameObject promptCanvas;
    [Tooltip("Speed at which canvas turns to face player")]
    public float smoothSpeed = 5;
    [Tooltip("Button player must press to interact")]
    public KeyCode interactKey;

    float promptYVelocity;

    bool hasPlayer = false; // Whether player is inside interaction range
    Transform playerCamera;
    bool canInteract = true; // Whether player can interact or not
    bool isFacing = false; // Whether player is facing interactable or not
    bool interacting = false; // Whether player is currently interacting

    public virtual void Start()
    {
        playerCamera = GameObject.FindWithTag("MainCamera").transform;
        promptCanvas.SetActive(false);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canInteract && !interacting)
        {
            hasPlayer = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && canInteract)
        {
            hasPlayer = false;
            promptCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        
        if (hasPlayer && canInteract && !interacting)
        {
            promptCanvas.SetActive(true);
            isFacing = true;

            // Make prompt canvas face player
            float targetYRot = playerCamera.eulerAngles.y;
            float currentYRot = promptCanvas.transform.rotation.eulerAngles.y;
            float newYAngle = Mathf.SmoothDampAngle(currentYRot, targetYRot, ref promptYVelocity, Time.deltaTime * smoothSpeed);
            float xAngle = playerCamera.eulerAngles.x;
            promptCanvas.transform.eulerAngles = new Vector3(xAngle, newYAngle, promptCanvas.transform.eulerAngles.z);  
        }

        if (!interacting && canInteract && Input.GetKeyDown(interactKey) && hasPlayer && isFacing)
        {
            Interact();
            promptCanvas.SetActive(false);
        }
    }

    public virtual void Interact()
    {
        if (!interacting)
        {
            interacting = true;
            interactEvent.Invoke();
            Debug.Log("Interacted with " + gameObject.name);
        }
        else
        {
            interacting = false;
            leaveEvent.Invoke();
            Debug.Log("Stopping interaction");
        }
    }

    public virtual void SetInteractableActive(bool state)
    {
        canInteract = state;
    }
}
