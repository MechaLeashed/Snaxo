using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CombatUtility - Static helper class with common combat-related utility functions.
/// Provides: Distance calculations, line of sight checks, target detection, damage math.
/// </summary>
public static class CombatUtility
{
    /// <summary>Checks if a target is within range of an origin point.</summary>
    public static bool IsTargetInRange(Vector3 origin, Vector3 target, float range)
    {
        return Vector3.Distance(origin, target) <= range;
    }

    /// <summary>Finds all entities with HealthSystem within a given range.</summary>
    public static List<HealthSystem> FindEntitiesInRange(Vector3 origin, float range, GameObject excludeGameObject = null)
    {
        List<HealthSystem> entitiesInRange = new List<HealthSystem>();

        Collider[] collidersInRange = Physics.OverlapSphere(origin, range);

        foreach (Collider collider in collidersInRange)
        {
            if (excludeGameObject != null && collider.gameObject == excludeGameObject)
                continue;

            HealthSystem health = collider.GetComponent<HealthSystem>();
            if (health != null && !health.IsDead())
            {
                entitiesInRange.Add(health);
            }
        }

        return entitiesInRange;
    }

    /// <summary>Checks if there's a clear line of sight from origin to target.</summary>
    public static bool HasLineOfSight(Vector3 origin, Vector3 target, float maxDistance = Mathf.Infinity)
    {
        Vector3 directionToTarget = target - origin;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > maxDistance)
            return false;

        Ray ray = new Ray(origin, directionToTarget.normalized);
        
        if (Physics.Raycast(ray, out RaycastHit hit, distanceToTarget))
        {
            return hit.collider.GetComponent<HealthSystem>() != null;
        }

        return false;
    }

    /// <summary>Calculates the direction from one point to another.</summary>
    public static Vector3 GetDirectionToTarget(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    /// <summary>Calculates distance between two points.</summary>
    public static float GetDistanceToTarget(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }

    /// <summary>Applies a percentage-based damage reduction (for armor, resistances).</summary>
    public static float ApplyDamageReduction(float baseDamage, float reductionPercent)
    {
        float reductionFactor = Mathf.Max(0, 1f - (reductionPercent / 100f));
        return baseDamage * reductionFactor;
    }

    /// <summary>Applies a multiplier-based damage bonus (for critical hits, buffs).</summary>
    public static float ApplyDamageMultiplier(float baseDamage, float multiplier)
    {
        return baseDamage * multiplier;
    }

    /// <summary>Checks if a random event occurs based on percentage chance.</summary>
    public static bool RollForChance(float chancePercent)
    {
        return Random.value * 100f <= chancePercent;
    }

    /// <summary>Gets the closest entity from a list of health systems.</summary>
    public static HealthSystem GetClosestEntity(Vector3 origin, List<HealthSystem> entities)
    {
        if (entities.Count == 0)
            return null;

        HealthSystem closestEntity = null;
        float closestDistance = float.MaxValue;

        foreach (HealthSystem entity in entities)
        {
            float distance = Vector3.Distance(origin, entity.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEntity = entity;
            }
        }

        return closestEntity;
    }

    /// <summary>Draws a debug visualization of an area-of-effect radius.</summary>
    public static void DrawAoEDebug(Vector3 position, float radius, Color color)
    {
        Debug.DrawLine(position + Vector3.up * radius, position - Vector3.up * radius, color);
        Debug.DrawLine(position + Vector3.right * radius, position - Vector3.right * radius, color);
        Debug.DrawLine(position + Vector3.forward * radius, position - Vector3.forward * radius, color);
    }
}
