using UnityEngine;

public class AttackVisual : MonoBehaviour
{
    public float duration = 0.2f;

    void Start()
    {
        Destroy(gameObject, duration);
    }
}