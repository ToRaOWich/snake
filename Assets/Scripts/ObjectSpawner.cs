using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{

    public GameObject applePrefab;
    public float appleSpawnInterval = 3f;
    public float minAppleSpawnDistance = 5f;
    public int maxApples = 10;
    private List<GameObject> spawnedApples = new List<GameObject>();
    private float appleSpawnTimer = 0f;
   
    public GameObject obstaclePrefab;

    public float obstacleSpawnInterval = 5f;
    public float minObstacleSpawnDistance = 7f;
    public int maxObstacles = 5;
    private List<GameObject> spawnedObstacles = new List<GameObject>();
    private float obstacleSpawnTimer = 0f;

    public Transform snakeHeadTransform;
    public Vector3 spawnAreaSize = new Vector3(20f, 1f, 20f);

    void Update()
    {
        appleSpawnTimer += Time.deltaTime;
        if (appleSpawnTimer >= appleSpawnInterval)
        {
            SpawnApple();
            appleSpawnTimer = 0f;
        }

        obstacleSpawnTimer += Time.deltaTime;
        if (obstacleSpawnTimer >= obstacleSpawnInterval)
        {
            SpawnObstacle();
            obstacleSpawnTimer = 0f;
        }

        CheckAndDestroyExcessObjects(spawnedApples, maxApples);
        CheckAndDestroyExcessObjects(spawnedObstacles, maxObstacles);
    }

    void SpawnApple()
    {
        if (applePrefab == null || snakeHeadTransform == null) return;

        Vector3 randomPosition;
        int maxAttempts = 10; // ป้องกันการวนลูปไม่สิ้นสุด

        for (int i = 0; i < maxAttempts; i++)
        {
            randomPosition = GetRandomSpawnPosition();
            if (Vector3.Distance(randomPosition, snakeHeadTransform.position) >= minAppleSpawnDistance)
            {
                GameObject newApple = Instantiate(applePrefab, randomPosition, Quaternion.identity);
                spawnedApples.Add(newApple);
                return;
            }
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null || snakeHeadTransform == null) return;

        Vector3 randomPosition;
        int maxAttempts = 10; // ป้องกันการวนลูปไม่สิ้นสุด

        for (int i = 0; i < maxAttempts; i++)
        {
            randomPosition = GetRandomSpawnPosition();
            if (Vector3.Distance(randomPosition, snakeHeadTransform.position) >= minObstacleSpawnDistance)
            {
                GameObject newObstacle = Instantiate(obstaclePrefab, randomPosition, Quaternion.identity);
                spawnedObstacles.Add(newObstacle);
                return;
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(transform.position.x - spawnAreaSize.x / 2f, transform.position.x + spawnAreaSize.x / 2f);
        float randomZ = Random.Range(transform.position.z - spawnAreaSize.z / 2f, transform.position.z + spawnAreaSize.z / 2f);
        return new Vector3(randomX, 0.5f, randomZ); // สมมติว่าเกิดบนระนาบ Y = 0.5f
    }

    void CheckAndDestroyExcessObjects(List<GameObject> objectList, int maxCount)
    {
        if (objectList.Count > maxCount)
        {
            if (objectList[0] != null)
            {
                Destroy(objectList[0]);
            }
            objectList.RemoveAt(0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}