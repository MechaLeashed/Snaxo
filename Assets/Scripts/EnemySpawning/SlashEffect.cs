using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float duration = 0.2f;
    public float rotationSpeed = 720f;
    public float growSpeed = 5f;

    private float timer;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float scaleMultiplier = Mathf.Sin((timer / duration) * Mathf.PI);
        transform.localScale = initialScale * (1 + scaleMultiplier);

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}