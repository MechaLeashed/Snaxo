using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] plantPrefabs;

    public Transform ground;
    public float spawnHeight = 0.5f;

    public int enemyCount = 10;
    public int plantCount = 20;

    public float minDistanceFromPlayer = 5f;
    public Transform player;

    void Start()
    {
        SpawnEnemies();
        SpawnPlants();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = GetRandomPosition();

            if (Vector3.Distance(pos, player.position) < minDistanceFromPlayer)
            {
                i--;
                continue;
            }

            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], pos, Quaternion.identity);
        }
    }

    void SpawnPlants()
    {
        for (int i = 0; i < plantCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            Instantiate(plantPrefabs[Random.Range(0, plantPrefabs.Length)], pos, Quaternion.identity);
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 size = ground.localScale * 10f;

        float randomX = Random.Range(-size.x / 2, size.x / 2);
        float randomZ = Random.Range(-size.z / 2, size.z / 2);

        Vector3 worldPos = ground.position + new Vector3(randomX, spawnHeight, randomZ);

        return worldPos;
    }
}