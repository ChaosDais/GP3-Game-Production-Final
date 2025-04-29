using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class FlyingEnemyBehavior : DamageableCharacter
{
    [Header("Navigation")]
    public float flyingHeight = 5f;
    private Vector3 randomTarget;
    private float wanderRadius = 15f;
    private float wanderThreshold = 1f;
    private float wanderSpeed = 1f;

    [Header("Detection")]
    public float detectionRadius = 10f;

    [Header("Combat")]
    public bool isRanged = true; // Toggle between ranged and melee
    public GameObject projectilePrefab;
    public float shootingInterval = 3f;
    private float shootingTimer;
    public Transform projectileSpawnPoint;

    // Melee attack fields
    private float meleeAttackTimer = 0f;
    private float meleeAttackDelay = 2f;
    private float meleeAttackRange = 3f;
    private float meleeDamageRadius = 6f;
    private bool hasDealtMeleeDamage = false;
    private bool hasExploded = false;
    private bool isRushing = false;
    private Vector3 lockedTargetPosition;
    public int blowupDam = 5;

    private Transform player;
    vThirdPersonController playerController;
    private bool playerInRange;

    public override void Start()
    {
        base.Start();

        randomTarget = GetRandomWanderPoint();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        shootingTimer = shootingInterval;

        if (player != null)
        {
            playerController = player.GetComponent<vThirdPersonController>();
        }
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        // Check player detection
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        playerInRange = distToPlayer <= detectionRadius && (!playerController.hidden);

        // Maintain flying height
        Vector3 targetPosition = transform.position;
        targetPosition.y = flyingHeight;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);

        if (playerInRange)
        {
            transform.LookAt(player);

            if (isRanged)
            {
                shootingTimer -= Time.deltaTime;
                if (shootingTimer <= 0)
                {
                    ShootAtPlayer();
                    shootingTimer = shootingInterval;
                }
            }
            else // Melee logic
            {
                if (!isRushing)
                {
                    // Lock player position and begin rush
                    lockedTargetPosition = player.position;
                    lockedTargetPosition.y = transform.position.y; // stay level
                    isRushing = true;
                    meleeAttackTimer = 0f;
                    hasDealtMeleeDamage = false;
                }

                // Move toward the locked target position
                if (Vector3.Distance(transform.position, lockedTargetPosition) > 0.5f)
                {
                    Vector3 rushDir = (lockedTargetPosition - transform.position).normalized;
                    transform.position += rushDir * (wanderSpeed * 10f) * Time.deltaTime;
                }
                else
                {
                    // At target, begin countdown to explode
                    meleeAttackTimer += Time.deltaTime;
                    if (meleeAttackTimer >= meleeAttackDelay && !hasDealtMeleeDamage)
                    {
                        DealMeleeDamage();
                        hasDealtMeleeDamage = true;
                        hasExploded = true;
                    }
                }
            }
        }
        else if (!isRanged && !isRushing)
        {
            // Wandering logic (only when not chasing)
            if (Vector3.Distance(transform.position, randomTarget) < wanderThreshold)
            {
                randomTarget = GetRandomWanderPoint();
            }

            Vector3 moveDirection = (randomTarget - transform.position);
            moveDirection.y = 0;
            if (moveDirection.magnitude > 0.1f)
            {
                moveDirection = moveDirection.normalized;
                transform.position += moveDirection * wanderSpeed * Time.deltaTime;
            }
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab != null && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + direction;
            spawnPos.y += 1.2f;

            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

            if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(direction * 100f, ForceMode.Impulse);
            }
        }
    }

    private Vector3 GetRandomWanderPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 point = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        point.y = flyingHeight;
        return point;
    }

    private void DealMeleeDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, meleeDamageRadius);
        foreach (var hitCollider in hitColliders)
        {
            DamageableCharacter playerHealth = hitCollider.gameObject.GetComponent<DamageableCharacter>();
            if (playerHealth != null && hitCollider.gameObject.CompareTag("Player"))
            {
                playerHealth.OnHit(blowupDam); // Adjust damage as needed
            }
        }


        Destroy(gameObject); // Self-destruct
    }
}
