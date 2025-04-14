using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Stealable : Interactable
{
    [Tooltip("No spaces.")]
    public string abilityName;

    Outline outline;
    AbilitySteal player;

    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<AbilitySteal>();
        if(GetComponent<Outline>() != null)
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
        if (CompareTag("ArcaneSoul") && other.CompareTag("Player"))
        {
            outline.enabled = true;
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        
        if (other.CompareTag("Player"))
        {
            outline.enabled = false;
        }
    }

    public void GiveAbility()
    {
        player.GainAbility(abilityName);
    }
    public void DestroySoul()
    {
        Destroy(gameObject);
    }
}
