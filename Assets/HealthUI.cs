using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] public Image healthBarImage;
    [SerializeField] public DamageableCharacter character;

    private void Update()
    {
        // Update the health bar fill amount based on current health percentage
        if (character != null && healthBarImage != null)
        {
            float healthPercentage = (float)character.Health / character.maxHealth;
            healthBarImage.fillAmount = healthPercentage;
        }
    }
}
