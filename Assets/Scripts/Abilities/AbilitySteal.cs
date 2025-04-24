using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class AbilitySteal : MonoBehaviour
{
    public GameObject abilityUI;
    public enum Ability
    {
        None,
        FastDash,
        FireBall,
        GhostStep
    }

    private Ability currentAbility;
    private Ability newAbility;

    private void Awake()
    {
        currentAbility = Ability.None;
        newAbility = currentAbility;

        gameObject.GetComponent<FastDash>().enabled = false;
        gameObject.GetComponent<FireBall>().enabled = false;
        gameObject.GetComponent<GhostStep>().enabled = false;
    }

    public void GainAbility(string name)
    {
        // Convert Arcane Soul's name into a matching enum
        if (Enum.TryParse(name, out newAbility))
        {
            //Debug.Log("Parsed successfully.");
            //Debug.Log("After Parsing, current ability is " + currentAbility + " and new ability is " + newAbility);
        } 
        else
        {
            Debug.LogError("Parse unsuccessful.");
        }

        if (currentAbility == Ability.None)
        {
            //Debug.Log("Currently don't have an ability; adding script.");
            AddProperAbility(newAbility);
        }
        else if (currentAbility != newAbility)
        {
            //Debug.Log("Already have " + currentAbility + ". Replacing with " + newAbility);
            PurgeCurrentAbility();
            AddProperAbility(newAbility);
        } else
        {
            Debug.LogWarning("Trying to change ability to " + newAbility + ", which is the same as the current ability " + currentAbility);
        }
    }

    void AddProperAbility(Ability abil)
    {
        // Add the new ability's script to the player
        if(abil != Ability.None) abilityUI.transform.GetChild(3).gameObject.SetActive(false);

        switch (abil)
        {
            case Ability.FastDash:
                abilityUI.transform.GetChild(0).gameObject.SetActive(true);
                gameObject.GetComponent<FastDash>().enabled = true;
                currentAbility = Ability.FastDash;
                break;
            case Ability.FireBall:
                abilityUI.transform.GetChild(1).gameObject.SetActive(true);
                gameObject.GetComponent<FireBall>().enabled = true;
                currentAbility = Ability.FireBall;
                break;
            case Ability.GhostStep:
                abilityUI.transform.GetChild(2).gameObject.SetActive(true);
                gameObject.GetComponent<GhostStep>().enabled = true;
                currentAbility = Ability.GhostStep;
                break;
            default:
                currentAbility = Ability.None;
                break;
        }
    }

    void PurgeCurrentAbility()
    {
        // Remove the current ability's script from the player
        switch (currentAbility)
        {
            case Ability.FastDash:
                abilityUI.transform.GetChild(0).gameObject.SetActive(false);
                gameObject.GetComponent<FastDash>().enabled = false;
                break;
            case Ability.FireBall:
                abilityUI.transform.GetChild(1).gameObject.SetActive(false);
                gameObject.GetComponent<FireBall>().enabled = false;
                break;
            case Ability.GhostStep:
                abilityUI.transform.GetChild(2).gameObject.SetActive(false);
                gameObject.GetComponent<GhostStep>().enabled = false;
                break;
            default:
                break;
        }
    }
}
