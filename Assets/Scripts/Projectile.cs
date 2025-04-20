using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 5;          
    public float lifetime = 5f; 

    void Start()
    {
        
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player
        DamageableCharacter playerHealth = collision.gameObject.GetComponent<DamageableCharacter>();
        if (playerHealth != null && collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to player
            playerHealth.OnHit(damage);
        }
        Destroy(gameObject);
    }
}
