using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class LeverInteract : Interactable
{
    [SerializeField] private Animator lever;
    [SerializeField] private Animator bridge;
    Outline outline;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();


        if (GetComponent<Outline>() != null)
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }
        else
        {
            Debug.LogError("Attach the Outline script to this object!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (CompareTag("Lever") && other.CompareTag("Player"))
        {
            outline.enabled = true;
        }
    }
    // Update is called once per frame
    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.CompareTag("Player"))
        {
            outline.enabled = false;
        }
    }

    public void PullLever()
    {
        lever.SetBool("IsPulled", true);
        bridge.SetBool("BridgeExpanded", true);
    }

}
