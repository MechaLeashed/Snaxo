using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] public float maxHealth = 100f;

    public ObjectPooler pooler;

    protected void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        // Reduce current health by damage amount
        currentHealth -= damage;
        Debug.Log("Damage taken: " + damage + ". Current health: " + currentHealth);

        // Checks if health is zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        if (pooler != null)
        {
            pooler.ReturnObject(gameObject);
            Debug.Log("Entity is dead and returned to pool.");
        }
        else
        {
            Debug.LogWarning("No pooler assigned for this enemy!");
            gameObject.SetActive(false);
        }

        Spawner spawner = FindFirstObjectByType<Spawner>();
        if (spawner != null)
        {
            spawner.OnEnemyKilled();
        }
    }

    private void OnEnable() // Called when object is enabled in pool
    {
        // Resets HP
        currentHealth = maxHealth;
    }
}
