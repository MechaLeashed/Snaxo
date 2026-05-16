using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour {
    //States
    public enum EnemyState {
        Idle,
        Patrol,
        Attack
    }
    public EnemyState currentState = EnemyState.Patrol;

    // References
    public Transform Player;
    public Transform[] patrolPoints;
    private NavMeshAgent agent;

    // Settings
    public float detectionRadius = 10f;
    public float attackRange = 3f;
    public float idleWaitTime = 2f;

    // Internal 
    private int currentPatrolIndex = 0;
    private float idleTimer = 0f;
    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer <= attackRange) {
            currentState = EnemyState.Attack;
        } else if (distanceToPlayer <= detectionRadius) {
            currentState = EnemyState.Attack;
        }


        switch (currentState) {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    void Idle() {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleWaitTime) {
            idleTimer = 0f;
            currentState = EnemyState.Patrol;
        }
    }

    void Patrol() {
        if (patrolPoints.Length == 0)
            return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.5f) {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentState = EnemyState.Idle;
        }

    }

    void Attack() {
        agent.SetDestination(Player.position);

        if (Vector3.Distance(transform.position, Player.position) > detectionRadius) {
            currentState = EnemyState.Patrol;
        }

        if (Vector3.Distance(transform.position, Player.position) <= attackRange) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}