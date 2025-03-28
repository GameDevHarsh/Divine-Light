using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public bool wanderOnX = true; // Determines if AI moves on X or Y axis
    public float movementDistance = 10f; // Max distance to move before turning back
    public float raycastDistance = 5f;
    public float attackRange = 1f;
    public LayerMask playerLayer; // Assign this in the Inspector

    private NavMeshAgent agent;
    private Transform player;
    private Vector2 startPoint;
    private Vector2 targetPoint;
    private int direction = 1; // 1 for forward, -1 for backward

    private enum AIState { Patrol, Chase, Attack, Retreat }
    private AIState currentState = AIState.Patrol;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startPoint = transform.position;
        CalculateNextPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
            case AIState.Retreat:
                Retreat();
                break;
        }

        FlipSprite();
    }

    void Patrol()
    {
        if (DetectPlayer())
        {
            currentState = AIState.Chase;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            direction *= -1; // Reverse direction
            CalculateNextPatrolPoint();
        }
    }

    void Chase()
    {
        if (!DetectPlayer())
        {
            currentState = AIState.Retreat;
            return;
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (!DetectPlayer() || Vector2.Distance(transform.position, player.position) > attackRange)
        {
            currentState = AIState.Chase;
            return;
        }

        // Attack logic (e.g., damage player, play animation)
    }

    void Retreat()
    {
        agent.SetDestination(startPoint);

        if (Vector2.Distance(transform.position, startPoint) < 0.1f)
        {
            currentState = AIState.Patrol;
            CalculateNextPatrolPoint();
        }
    }

    void CalculateNextPatrolPoint()
    {
        float step = movementDistance * direction;
        Vector2 testPoint = wanderOnX ? new Vector2(startPoint.x + step, startPoint.y) : new Vector2(startPoint.x, startPoint.y + step);

        // Reduce movement distance if destination is not reachable
        while (!NavMesh.SamplePosition(testPoint, out _, 0.5f, NavMesh.AllAreas) && movementDistance > 1f)
        {
            movementDistance -= 1f;
            step = movementDistance * direction;
            testPoint = wanderOnX ? new Vector2(startPoint.x + step, startPoint.y) : new Vector2(startPoint.x, startPoint.y + step);
        }

        targetPoint = testPoint;
        agent.SetDestination(targetPoint);
    }

    bool DetectPlayer()
    {
        Vector2 directionVector = wanderOnX ? Vector2.right * Mathf.Sign(agent.velocity.x) : Vector2.up * Mathf.Sign(agent.velocity.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionVector, raycastDistance, playerLayer);

        Debug.DrawRay(transform.position, directionVector * raycastDistance, Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    void FlipSprite()
    {
        Vector2 moveDirection = agent.velocity.normalized;

        if (wanderOnX)
        {
            spriteRenderer.flipX = moveDirection.x < 0;
        }
        else
        {
            if (moveDirection.y > 0)
                transform.rotation = Quaternion.Euler(0, 0, 90);
            else if (moveDirection.y < 0)
                transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }
}
