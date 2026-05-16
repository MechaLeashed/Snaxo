using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;

    public GameObject[] dropItems;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        DropItems();
        Destroy(gameObject);
    }

    void DropItems()
    {
        if (dropItems.Length == 0) return;

        int dropIndex = Random.Range(0, dropItems.Length);

        Instantiate(dropItems[dropIndex], transform.position, Quaternion.identity);
    }
}