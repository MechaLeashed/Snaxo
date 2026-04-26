using UnityEngine;
using UnityEngine.AI;

public class PooledEnemyInitializer : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        if (agent != null)
        {
            agent.enabled = false;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }

            agent.enabled = true;
            agent.ResetPath();
        }
    }
}