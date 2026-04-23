using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyCombatSystem - Handles AI-controlled enemy combat behavior.
/// Responsibilities: Detect player, chase/attack, manage AI state machine.
/// </summary>
public class EnemyCombatSystem : MonoBehaviour
{
    public enum AIState { Idle, Chasing, Attacking, Dead }

    private HealthSystem healthSystem;
    private DamageSystem damageSystem;
    private CharacterController characterController;

    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float chaseSpeed = 4f;

    private AIState currentState = AIState.Idle;
    private Transform targetPlayer;
    private float lastAttackTime = 0f;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        damageSystem = GetComponent<DamageSystem>();
        characterController = GetComponent<CharacterController>();

        if (healthSystem != null)
            healthSystem.OnDeath += OnDeath;

        if (targetPlayer == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                targetPlayer = playerObj.transform;
        }
    }

    void Update()
    {
        if (currentState == AIState.Dead)
            return;

        UpdateAIState();

        switch (currentState)
        {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Chasing:
                Chase();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }
    }

    private void UpdateAIState()
    {
        if (targetPlayer == null)
        {
            currentState = AIState.Idle;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRange)
            currentState = AIState.Attacking;
        else if (distanceToPlayer <= detectionRange)
            currentState = AIState.Chasing;
        else
            currentState = AIState.Idle;
    }

    private void Idle() { }

    private void Chase()
    {
        if (targetPlayer == null || characterController == null)
            return;

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;

        if (characterController.enabled)
        {
            characterController.Move(directionToPlayer * chaseSpeed * Time.deltaTime);
        }

        transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    private void Attack()
    {
        if (targetPlayer == null || damageSystem == null)
            return;

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            HealthSystem playerHealth = targetPlayer.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsDead())
            {
                damageSystem.DealDamage(playerHealth, DamageSystem.DamageType.Physical);
                Debug.Log("Enemy attacks player!");
            }
        }
    }

    private void OnDeath()
    {
        currentState = AIState.Dead;

        if (characterController != null)
            characterController.enabled = false;

        Debug.Log($"{gameObject.name} has died!");
    }

    public void SetTarget(Transform newTarget) => targetPlayer = newTarget;
    public void SetDetectionRange(float range) => detectionRange = range;
    public void SetAttackRange(float range) => attackRange = range;
    public AIState GetCurrentState() => currentState;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
