using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : DamageableCharacter
{
    public Transform[] patrolPoints;
    
    public float chaseRadius = 10f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    Transform player;

    public override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").transform;
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