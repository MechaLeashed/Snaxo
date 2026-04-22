using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthBar health = other.GetComponent<HealthBar>();

        if (health != null)
        {
            health.takeDamage(damage);
        }
    }
}