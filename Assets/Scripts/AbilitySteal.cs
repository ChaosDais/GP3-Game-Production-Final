using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilitySteal : MonoBehaviour
{
    public TextMeshProUGUI abilityText;
    public void GainAbility(string name)
    {
        abilityText.text = name;
    }
}
