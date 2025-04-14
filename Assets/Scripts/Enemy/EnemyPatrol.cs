using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : DamageableCharacter
{
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRadius = 10f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PatrolNextPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= chaseRadius)
        {
            ChasePlayer();
        }
        else if (isChasing)
        {
            isChasing = false;
            PatrolNextPoint();
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isChasing)
        {
            PatrolNextPoint();
        }
    }

    void PatrolNextPoint()
    {
        if (patrolPoints.Length == 0)
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