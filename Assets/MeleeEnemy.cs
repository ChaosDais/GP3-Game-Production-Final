using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public float attackRange = 2f;         // Range at which enemy can attack
    public float attackCooldown = 3f;      // Cooldown between attacks in seconds
    public int damageAmount = 5;        // Amount of damage to deal
    public string targetTag = "Player";     // Tag of objects to damage

    private float nextAttackTime;          // Time when next attack is allowed
    private Animator animator;             // Reference to the animator component
    private Transform player;              // Reference to the player's transform
    private SphereCollider attackCollider; // Reference to the sphere collider for attacks

    void Start()
    {
        // Get reference to the animator component
        animator = GetComponent<Animator>();

        // Find the player object
        player = GameObject.FindGameObjectWithTag(targetTag)?.transform;

        // Initialize next attack time
        nextAttackTime = 0f;

        // Setup attack collider
        attackCollider = gameObject.AddComponent<SphereCollider>();
        attackCollider.radius = attackRange;
        attackCollider.isTrigger = true;
        attackCollider.enabled = false; // Only enable during attacks
    }

    void Update()
    {
        if (player == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Set attack state based on distance
        if (distanceToPlayer <= attackRange)
        {
            // Only attack if cooldown has expired
            if (Time.time >= nextAttackTime)
            {
                Attack();
            }
        }
        else
        {
            // Player is too far, set attack to false
            animator.SetBool("IsAttacking", false);
        }
    }

    void Attack()
    {
        // Set attack animation state to true
        animator.SetBool("IsAttacking", true);

        // Enable the attack collider
        attackCollider.enabled = true;

        // Check for hits
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hitCollider in hitColliders)
        {
            DamageableCharacter playerHealth = hitCollider.gameObject.GetComponent<DamageableCharacter>();
            if (playerHealth != null && hitCollider.gameObject.CompareTag(targetTag))
            {
                playerHealth.OnHit(damageAmount);
            }
        }

        // Disable the attack collider after checking
        attackCollider.enabled = false;

        // Set next attack time
        nextAttackTime = Time.time + attackCooldown;
    }
}
