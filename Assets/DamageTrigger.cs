using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public int turretDamage = 10;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            DamageableCharacter playerHealth = other.gameObject.GetComponent<DamageableCharacter>();
            if (playerHealth != null)
            {
                
                playerHealth.OnHit(turretDamage); 
            }
        }
    }
}

