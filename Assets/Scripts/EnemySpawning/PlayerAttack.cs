using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 0.5f;

    public Transform attackPoint;
    public LayerMask targetLayers;

    public GameObject attackVisual;

    private float lastAttackTime;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        if (attackVisual != null)
        {
            Instantiate(attackVisual, attackPoint.position, transform.rotation);
        }

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, targetLayers);

        foreach (Collider hit in hits)
        {
            Damageable dmg = hit.GetComponent<Damageable>();

            if (dmg != null)
            {
                dmg.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}