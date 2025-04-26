using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 5;
    public float lifetime = 5f;
    public float attackRange = 2f; // Radius for OverlapSphere
    public string targetTag = "Player"; // Tag to filter targets

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optional: You can add a visual or sound effect here before damage

        // Apply damage to all in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hitCollider in hitColliders)
        {
            DamageableCharacter playerHealth = hitCollider.gameObject.GetComponent<DamageableCharacter>();
            if (playerHealth != null && hitCollider.gameObject.CompareTag(targetTag))
            {
                playerHealth.OnHit(damage);
            }
        }

        Destroy(gameObject);
    }
}