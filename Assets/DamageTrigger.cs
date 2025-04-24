using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public TurretBehavior turret;
    public int turretDamage = 10;
    [SerializeField] private int laserDamage = 10;
    [SerializeField] private float damageInterval = 3f;
    private float lastDamageTime = -Mathf.Infinity;
    private void OnTriggerStay(Collider other)
    {
        if (Time.time - lastDamageTime >= damageInterval)
        {
            DamageableCharacter playerHealth = other.gameObject.GetComponent<DamageableCharacter>();
            if (playerHealth != null)
            {

                playerHealth.OnHit(turretDamage);
            }
            lastDamageTime = Time.time;
        }
        {
            
            
        }
    }
}

