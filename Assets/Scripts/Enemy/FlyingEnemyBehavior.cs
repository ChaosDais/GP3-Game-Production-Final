using System.Collections;
using System.Collections.Generic;
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

    // Melee attack fields
    private float meleeAttackTimer = 0f;
    private float meleeAttackDelay = 2f;
    private float meleeAttackRange = 3f;
    private float meleeDamageRadius = 6f;
    private bool hasDealtMeleeDamage = false;

    private Transform player;
    private bool playerInRange;

    public override void Start()
    {
        base.Start();

        randomTarget = GetRandomWanderPoint();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        shootingTimer = shootingInterval;
    }

    void Update()
    {
        if (player == null) return;

        // Vector math detection
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        playerInRange = distToPlayer <= detectionRadius;

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
                float dist = Vector3.Distance(transform.position, player.position);
                if (dist > meleeAttackRange)
                {
                    // Rush toward player only if not in range
                    Vector3 rushDir = (player.position - transform.position);
                    rushDir.y = 0;
                    if (rushDir.magnitude > 0.1f)
                    {
                        rushDir = rushDir.normalized;
                        transform.position += rushDir * (wanderSpeed * 10f) * Time.deltaTime; // Move faster when rushing
                    }
                    meleeAttackTimer = 0f;
                    hasDealtMeleeDamage = false;
                }
                else
                {
                    // Stop moving and start the explosion timer
                    meleeAttackTimer += Time.deltaTime;
                    if (meleeAttackTimer >= meleeAttackDelay && !hasDealtMeleeDamage)
                    {
                        DealMeleeDamage();
                        hasDealtMeleeDamage = true;
                    }
                }
            }
        }
        else
        {
            // Wander randomly
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
            GameObject projectile = Instantiate(projectilePrefab, transform.position + direction, Quaternion.LookRotation(direction));

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
                // Deal damage to player
                playerHealth.OnHit(5); //
            }
        }

        // Destroy this enemy after dealing damage
        Destroy(gameObject);
    }
}
