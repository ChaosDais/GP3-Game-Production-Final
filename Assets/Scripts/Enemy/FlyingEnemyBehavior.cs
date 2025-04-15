using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemyBehavior : DamageableCharacter
{
    [Header("Navigation")]
    public NavMeshAgent agent;
    public float flyingHeight = 5f;

    [Header("Detection")]
    public SphereCollider detectionZone;
    public float detectionRadius = 10f;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public float shootingInterval = 3f;
    private float shootingTimer;

    private Transform player;
    private bool playerInRange;

    public override void Start()
    {
        base.Start();
       
        agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.baseOffset = flyingHeight;
            agent.height = 0.5f;
        }

       
        if (!detectionZone)
        {
            detectionZone = gameObject.AddComponent<SphereCollider>();
            detectionZone.isTrigger = true;
            detectionZone.radius = detectionRadius;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        shootingTimer = shootingInterval;
    }

    void Update()
    {
        if (player == null) return;

        
        Vector3 targetPosition = transform.position;
        targetPosition.y = flyingHeight;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);

        if (playerInRange)
        {
            transform.LookAt(player);

            
            shootingTimer -= Time.deltaTime;
            if (shootingTimer <= 0)
            {
                ShootAtPlayer();
                shootingTimer = shootingInterval;
            }

           
            if (agent)
            {
                agent.SetDestination(player.position);
            }
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab != null && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            GameObject projectile = Instantiate(projectilePrefab, transform.position + direction, Quaternion.LookRotation(direction));

            
            if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(direction * 50f, ForceMode.Impulse);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
