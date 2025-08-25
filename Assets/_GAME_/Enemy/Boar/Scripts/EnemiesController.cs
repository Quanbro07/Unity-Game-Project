using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class EnemiesController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    public float spawnRadius = 15f;      // bán kính spawn quanh player
    public float minDistance = 5f;       // khoảng cách tối thiểu với player
    public int maxEnemies = 20;          // số lượng quái tối đa tồn tại
    public float spawnInterval = 2f;     // thời gian giữa mỗi lần spawn

    private float lastSpawnTime;
    private List<GameObject> spawnedEnemies = new List<GameObject>();


    void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        if (player == null) return;

        // Xoá quái đã chết khỏi list
        spawnedEnemies.RemoveAll(e => e == null);

        // Nếu chưa đủ số lượng quái và đã đến thời gian spawn mới
        if (spawnedEnemies.Count < maxEnemies && Time.time - lastSpawnTime >= spawnInterval)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    void SpawnEnemy()
    {
        // Random vị trí quanh player
        Vector2 spawnPos;
        int tries = 0;
        do
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Random.Range(minDistance, spawnRadius);
            spawnPos = (Vector2)player.position + randomOffset;
            tries++;
        }
        while (Vector2.Distance(spawnPos, player.position) < minDistance && tries < 10);

        // Chọn prefab random
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Spawn
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        var aiSetter = enemy.GetComponent<AIDestinationSetter>();
        if (aiSetter != null)
        {
            aiSetter.target = player; // player là Transform
        }

        spawnedEnemies.Add(enemy);
    }
}
