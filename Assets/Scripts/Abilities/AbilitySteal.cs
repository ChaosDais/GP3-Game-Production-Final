using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class AbilitySteal : MonoBehaviour
{
    public TextMeshProUGUI abilityText;
    public enum Ability
    {
        None,
        FastDash,
        FireBall,
        GhostStep
    }

    private Ability currentAbility;
    private Ability newAbility;

    private void Start()
    {
        currentAbility = Ability.None;
        newAbility = currentAbility;
        abilityText.text = currentAbility.ToString();
    }

    public void GainAbility(string name)
    {
        Debug.Log("Current ability is " + currentAbility + " and new ability is " + newAbility);
        // Convert Arcane Soul's name into a matching enum
        if (Enum.TryParse(name, out newAbility))
        {
            Debug.Log("Parsed successfully.");
            Debug.Log("After Parsing, current ability is " + currentAbility + " and new ability is " + newAbility);
        } 
        else
        {
            Debug.LogError("Parse unsuccessful.");
        }

        if (currentAbility == Ability.None)
        {
            Debug.Log("Currently don't have an ability; adding script.");
            AddProperAbility(newAbility);
        }
        else if (currentAbility != newAbility)
        {
            Debug.Log("Already have " + currentAbility + ". Replacing with " + newAbility);
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
        Debug.Log("Adding ability " + abil);
        switch (abil)
        {
            case Ability.FastDash:
                abilityText.text = "Fast Dash";
                gameObject.AddComponent<FastDash>();
                currentAbility = Ability.FastDash;
                break;
            case Ability.FireBall:
                abilityText.text = "Fire Ball";
                gameObject.AddComponent<FireBall>();
                currentAbility = Ability.FireBall;
                break;
            case Ability.GhostStep:
                abilityText.text = "Ghost Step";
                gameObject.AddComponent<GhostStep>();
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
                Destroy(gameObject.GetComponent<FastDash>());
                break;
            case Ability.FireBall:
                Destroy(gameObject.GetComponent<FireBall>());
                break;
            case Ability.GhostStep:
                Destroy(gameObject.GetComponent<GhostStep>());
                break;
            default:
                break;
        }
    }
}
