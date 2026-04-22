using UnityEngine;

public class takedamage : MonoBehaviour
{
    public float damage = 10f;
    public HealthBar healthBar; // reference to your player's HealthBar

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (healthBar != null)
            {
                healthBar.takeDamage(damage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HealthBar health = collision.gameObject.GetComponent<HealthBar>();

        if (health != null)
        {
            health.takeDamage(damage);
        }
    }
}