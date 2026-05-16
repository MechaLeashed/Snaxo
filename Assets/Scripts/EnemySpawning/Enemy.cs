using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 3f;
    public float damage = 10f;

    private Vector3 moveDirection;
    private float timer;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeDirectionTime)
        {
            PickNewDirection();
            timer = 0;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void PickNewDirection()
    {
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        moveDirection = new Vector3(x, 0, z).normalized;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        HealthBar player = hit.gameObject.GetComponent<HealthBar>();

        if (player != null)
        {
            player.takeDamage(damage);
        }
    }
}