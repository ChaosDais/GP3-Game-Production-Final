using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : DamageableCharacter
{
    public Transform[] patrolPoints;

    public float chaseRadius = 10f;
    public bool isRanged = false; // Whether this is a ranged enemy

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isInRange = false; // Track if player is in detection range
    private Animator animator;
    Transform player;

    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        PatrolNextPoint();
    }

    void Update()
    {

        bool isAttacking = animator.GetBool("IsAttacking");


        if (isAttacking)
        {
            agent.isStopped = true;
            return;
        }
        else
        {
            animator.SetBool("IsIdle", false);
            agent.isStopped = false;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        bool wasInRange = isInRange;
        isInRange = distanceToPlayer <= chaseRadius;

        if (isInRange)
        {
            if (isRanged)
            {
                // cause range is true they will not chase the player to prioritze ranged attack
                agent.isStopped = true;
                transform.LookAt(player);
                isChasing = false;
                animator.SetBool("IsIdle", true); // Set idle animation when stopped
            }
            else
            {
                // Melee enemies chase the player
                ChasePlayer();
                animator.SetBool("IsIdle", false); // Set idle animation when stopped
            }
        }
        else
        {
            // Resume patrol when player is out of range
            agent.isStopped = false;
            if (wasInRange || isChasing)
            {
                isChasing = false;
                PatrolNextPoint();
            }
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isChasing)
        {
            PatrolNextPoint();
        }
    }

    void PatrolNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void ChasePlayer()
    {
        isChasing = true;
        agent.destination = player.position;
    }
}