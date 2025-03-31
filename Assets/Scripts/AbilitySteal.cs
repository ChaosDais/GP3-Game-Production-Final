using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilitySteal : MonoBehaviour
{
    private Outline outline;

    private void Start()
    {
        if(GetComponent<Outline>() != null)
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;
        }
        else
            Debug.LogError("Attach the Outline script to this object!");
    }

    private void OnTriggerStay(Collider other)
    {
        if (CompareTag("StealableEnemy") && other.CompareTag("Player"))
        {
            if (outline != null)
                outline.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            outline.enabled = false;
    }
}
