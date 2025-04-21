using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMelee : MonoBehaviour
{
    public float attackRange = 5f;         
    public float attackCooldown = 2f;      
    public GameObject projectilePrefab;    
    public Transform projectileSpawnPoint; 
    public float projectileSpeed = 10f;    
    public float targetHeightOffset = 1f; 

    private float nextAttackTime;
    private Animator animator;
    private Transform player;

    void Start()
    {

        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextAttackTime = 0f;
    }

    void Update()
    {
        if (player == null) return;


        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

   
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        // Set next attack time
        nextAttackTime = Time.time + attackCooldown;

        // attack animation
        animator.SetBool("IsAttacking", true);

        // Wait a bit for the animation
        yield return new WaitForSeconds(0.5f);

        // Spawn projectile
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

            
            Vector3 targetPosition = player.position + Vector3.up * targetHeightOffset;

           
            Vector3 directionToPlayer = (targetPosition - projectileSpawnPoint.position).normalized;

           
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * projectileSpeed;
            }
        }

        // Reset attack animation
        animator.SetBool("IsAttacking", false);
    }
}
