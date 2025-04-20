using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public float attackRange = 2f;         // Range at which enemy can attack
    public float attackCooldown = 3f;      // Cooldown between attacks in seconds

    private float nextAttackTime;          // Time when next attack is allowed
    private Animator animator;             // Reference to the animator component
    private Transform player;              // Reference to the player's transform

    void Start()
    {
        // Get reference to the animator component
        animator = GetComponent<Animator>();

        // Find the player object
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize next attack time
        nextAttackTime = 0f;
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

        // Set next attack time
        nextAttackTime = Time.time + attackCooldown;
    }
}
