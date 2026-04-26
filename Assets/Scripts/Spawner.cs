using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int enemiesPerWave = 15;
    [SerializeField] private float waveDelay = 5f;

    private int activeEnemies = 0;
    public ObjectPooler pooler;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        activeEnemies = enemiesPerWave;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject obj = pooler.GetObject();

            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            Vector3 spawnOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            obj.transform.position = transform.position + spawnOffset;

            if (agent != null)
            {
                agent.enabled = true;
                agent.Warp(obj.transform.position);
            }

            EnemyHealth health = obj.GetComponent<EnemyHealth>();
            if (health != null) health.pooler = pooler;
        }

        while (activeEnemies > 0) yield return null;

        yield return new WaitForSeconds(waveDelay);
        StartCoroutine(SpawnWave());
    }

    public void OnEnemyKilled()
    {
        activeEnemies--;
    }
}