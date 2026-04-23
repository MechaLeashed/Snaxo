using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DamageSystem - Simple damage calculation and application.
/// 
/// Responsibilities:
/// - Calculate and apply basic damage
/// - Support different damage types for future expansion (e.g., burn damage)
/// - Deal damage to target entities via their HealthSystem
/// - Track damage dealt (for statistics/feedback)
/// </summary>
public class DamageSystem : MonoBehaviour
{
    // Damage types for different attack methods (extensible for future systems)
    public enum DamageType
    {
        Physical,
        Magical,
        // Future: Fire, Ice, Burn, etc.
    }

    [SerializeField] private float baseDamage = 10f;

    public delegate void DamageDealtDelegate(float damageAmount, bool isCritical);
    public event DamageDealtDelegate OnDamageDealt;

    /// <summary>
    /// Deals damage to a target entity.
    /// </summary>
    /// <param name="targetHealth">The HealthSystem component of the target</param>
    /// <param name="damageType">Type of damage being dealt (for future use)</param>
    /// <param name="damageMultiplier">Additional multiplier for this specific attack</param>
    public void DealDamage(HealthSystem targetHealth, DamageType damageType, float damageMultiplier = 1f)
    {
        if (targetHealth == null || targetHealth.IsDead())
            return;

        // Calculate final damage: base * multiplier
        float finalDamage = baseDamage * damageMultiplier;
        finalDamage = Mathf.Max(0.1f, finalDamage); // Ensure minimum 0.1 damage

        // Apply damage to target
        targetHealth.TakeDamage(finalDamage);

        // Trigger event for feedback systems (UI, animations, particles)
        OnDamageDealt?.Invoke(finalDamage, false); // false for now, no crits

        // Log for debugging
        Debug.Log($"Dealt {finalDamage:F1} damage ({damageType})");
    }

    /// <summary>
    /// Sets the base damage for this entity.
    /// </summary>
    public void SetBaseDamage(float newDamage) => baseDamage = newDamage;

    /// <summary>
    /// Gets the current base damage value.
    /// </summary>
    public float GetBaseDamage() => baseDamage;
}
