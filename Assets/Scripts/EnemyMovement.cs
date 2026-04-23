using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float waitTimeAtWaypoint = 2f;
    public float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private PatrolPath patrolPath;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
            Debug.LogError("NavMeshAgent missing on " + gameObject.name);

        // Let script handle rotation manually for smoothness
        agent.updateRotation = false;
    }

    // Called by Architect's EnemyAI on Start
    public void SetPatrolPath(PatrolPath path) {
        patrolPath = path;
        isWaiting = false;
        waitTimer = 0f;

        if (patrolPath != null)
            agent.SetDestination(patrolPath.GetCurrentWaypoint());
    }

    // Called by Architect's EnemyAI every frame in Patrol state
    public void Patrol() {
        if (patrolPath == null) { Idle(); return; }

        agent.speed = patrolSpeed;

        if (isWaiting) {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint) {
                isWaiting = false;
                waitTimer = 0f;
                agent.SetDestination(patrolPath.GetNextWaypoint());
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            isWaiting = true;

        SmoothRotate(agent.velocity);
    }

    // Called by Architect's EnemyAI every frame in Chase state
    public void ChaseTarget(Transform target) {
        if (target == null)
            return;

        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);

        SmoothRotate(agent.velocity);
    }

    // Called by Architect's EnemyAI during Attack state
    public void StopMoving() {
        if (agent.isActiveAndEnabled)
            agent.ResetPath();
    }

    // Called by Architect's EnemyAI during Idle state
    public void Idle() {
        StopMoving();
    }

    // Called by Architect's EnemyAI when HP hits zero
    public void OnDeath() {
        StopMoving();
        agent.enabled = false;
    }

    // Called by Coordinator's EnemySpawner when resetting a pooled enemy
    public void ResetMovement() {
        agent.enabled = true;
        isWaiting = false;
        waitTimer = 0f;
    }

    public bool HasStopped() {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    // Smoothly rotates enemy to face movement direction
    private void SmoothRotate(Vector3 velocity) {
        if (velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 direction = new Vector3(velocity.x, 0f, velocity.z).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                               targetRotation,
                                               rotationSpeed * Time.deltaTime);
    }
}