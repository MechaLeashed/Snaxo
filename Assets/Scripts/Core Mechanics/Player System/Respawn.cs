using UnityEngine;
using UnityEngine.InputSystem;

public class Respawn : MonoBehaviour
{
    public HealthBar healthBar;
    public Transform respawnPoint;

    private Renderer[] renderers;
    private Collider[] colliders;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponents<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar != null && healthBar.health <= 0)
        {
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }

            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                transform.position = respawnPoint.position;
                healthBar.health = healthBar.maxHealth;

                foreach (Renderer r in renderers)
                {
                    r.enabled = true;
                }

                foreach (Collider c in colliders)
                {
                    c.enabled = true;
                }
            }
        }    
    }
}