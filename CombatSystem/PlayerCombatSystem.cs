using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerCombatSystem - Handles player-specific combat mechanics.
/// 
/// Responsibilities:
/// - Handle player attack input (mouse clicks, keyboard presses)
/// - Manage equipped weapons and attack selection
/// - Track attack cooldowns to prevent spam
/// - Manage combat state (attacking, cooldown, idle)
/// - Integrate with DamageSystem to deal damage
/// - Handle combo systems (if desired)
/// - Trigger attack animations
/// </summary>
public class PlayerCombatSystem : MonoBehaviour
{
    // Combat References
    private HealthSystem healthSystem;
    private DamageSystem damageSystem;

    // Attack state and cooldown
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    // Weapon configuration
    [SerializeField] private float weaponDamageMultiplier = 1f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private DamageSystem.DamageType attackDamageType = DamageSystem.DamageType.Physical;

    // Combo system
    private int comboCount = 0;
    [SerializeField] private int maxCombo = 3;
    [SerializeField] private float comboResetTime = 2f;
    private float lastComboTime = 0f;

    private bool isAttacking = false;

    // Events
    public delegate void AttackPerformedDelegate(bool isCombo, int comboNumber);
    public event AttackPerformedDelegate OnAttackPerformed;

    void Start()
    {
        // Get required components
        healthSystem = GetComponent<HealthSystem>();
        damageSystem = GetComponent<DamageSystem>();

        if (healthSystem == null)
            Debug.LogError("PlayerCombatSystem requires a HealthSystem component!");
        if (damageSystem == null)
            Debug.LogError("PlayerCombatSystem requires a DamageSystem component!");
    }

    void Update()
    {
        // Check for attack input (left mouse button or F key for combat)
        if (Keyboard.current.mouseButton0.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame)
        {
            AttemptAttack();
        }

        // Reset combo if enough time has passed
        if (Time.time - lastComboTime > comboResetTime && comboCount > 0)
        {
            comboCount = 0;
        }
    }

    /// <summary>
    /// Attempts to perform an attack if cooldown has elapsed.
    /// Searches for nearby enemies and deals damage to them.
    /// </summary>
    private void AttemptAttack()
    {
        // Check if enough time has passed since last attack
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;
        PerformAttack();
    }

    /// <summary>
    /// Executes the attack: finds targets and deals damage.
    /// </summary>
    private void PerformAttack()
    {
        // Find all enemies in range
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, attackRange);

        bool hitSomething = false;

        foreach (Collider collider in targetsInRange)
        {
            // Skip self
            if (collider.gameObject == gameObject)
                continue;

            // Check if target has HealthSystem (is an entity we can damage)
            HealthSystem targetHealth = collider.GetComponent<HealthSystem>();
            if (targetHealth != null && !targetHealth.IsDead())
            {
                // Deal damage using our DamageSystem
                damageSystem.DealDamage(targetHealth, attackDamageType, weaponDamageMultiplier);
                hitSomething = true;
            }
        }

        // Update combo system
        if (hitSomething)
        {
            lastComboTime = Time.time;
            comboCount++;

            // Cap combo at max
            if (comboCount > maxCombo)
                comboCount = maxCombo;

            OnAttackPerformed?.Invoke(true, comboCount);
            Debug.Log($"Attack! Combo: {comboCount}/{maxCombo}");
        }
        else
        {
            OnAttackPerformed?.Invoke(false, 0);
            Debug.Log("Attack missed!");
        }
    }

    /// <summary>
    /// Called when this player takes damage - can play hurt animation here.
    /// </summary>
    public void OnTakeDamage()
    {
        // Reset combo on taking damage
        comboCount = 0;
        Debug.Log("Hit! Combo reset.");
    }

    /// <summary>
    /// Sets the weapon damage multiplier for currently equipped weapon.
    /// </summary>
    public void SetWeaponDamage(float multiplier) => weaponDamageMultiplier = multiplier;

    /// <summary>
    /// Sets the attack range (for melee vs ranged weapons).
    /// </summary>
    public void SetAttackRange(float range) => attackRange = range;

    /// <summary>
    /// Sets the attack cooldown time between attacks.
    /// </summary>
    public void SetAttackCooldown(float cooldown) => attackCooldown = cooldown;

    /// <summary>
    /// Gets the current combo count.
    /// </summary>
    public int GetComboCount() => comboCount;

    /// <summary>
    /// Gets whether the player is currently in an attack animation.
    /// </summary>
    public bool IsAttacking() => isAttacking;

    /// <summary>
    /// Visualize attack range in editor for debugging.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
