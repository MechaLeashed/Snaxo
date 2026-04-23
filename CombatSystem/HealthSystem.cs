using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HealthSystem - Manages entity health/hitpoints and death state.
/// 
/// Responsibilities:
/// - Track current and maximum health values
/// - Handle damage taken and healing received
/// - Trigger death events when health reaches zero
/// - Prevent healing above max health
/// - Provide health percentage for UI and other systems
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    // Events for other systems to subscribe to
    public delegate void HealthChangedDelegate(float newHealth, float maxHealth);
    public delegate void DeathDelegate();

    public event HealthChangedDelegate OnHealthChanged;
    public event DeathDelegate OnDeath;

    private bool isDead = false;

    void Start()
    {
        // Initialize health to max on startup
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Takes damage and reduces health.
    /// Will not reduce health below 0.
    /// Triggers death event if health reaches 0.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to take</param>
    public void TakeDamage(float damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Trigger death if health is depleted
        if (currentHealth <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Heals the entity by restoring health.
    /// Will not exceed max health.
    /// </summary>
    /// <param name="healAmount">Amount of health to restore</param>
    public void Heal(float healAmount)
    {
        if (isDead)
            return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Revives the entity back to full health (used for respawning).
    /// </summary>
    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public float GetCurrentHealth() => currentHealth;

    /// <summary>
    /// Gets the maximum health value.
    /// </summary>
    public float GetMaxHealth() => maxHealth;

    /// <summary>
    /// Gets health as a percentage (0-1) for UI bars.
    /// </summary>
    public float GetHealthPercentage() => currentHealth / maxHealth;

    /// <summary>
    /// Checks if the entity is currently dead.
    /// </summary>
    public bool IsDead() => isDead;
}
