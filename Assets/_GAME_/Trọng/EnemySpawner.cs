using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs; // Kéo tất cả prefab quái vào đây

    [Header("Player Target")]
    [SerializeField] private Transform player;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private int maxEnemies = 10;      
    [SerializeField] private float pointCooldown = 8f;

    private Dictionary<Transform, float> lastSpawnTime = new Dictionary<Transform, float>();
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("⚠ Player chưa được gán trong EnemySpawner!");
            return;
        }
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("⚠ Chưa có spawn points nào!");
            return;
        }
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("⚠ Chưa gán enemyPrefabs trong Inspector!");
            return;
        }

        // Khởi tạo cooldown cho mỗi spawn point
        foreach (Transform point in spawnPoints)
        {
            lastSpawnTime[point] = -pointCooldown;
        }

        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Xóa quái đã chết khỏi danh sách
        activeEnemies.RemoveAll(e => e == null);

        // Kiểm tra giới hạn số lượng quái
        if (activeEnemies.Count >= maxEnemies)
        {
            Debug.Log("⚠ Số lượng quái đã đạt giới hạn, không spawn thêm!");
            return;
        }

        // Lọc spawn points gần player và hết cooldown
        List<Transform> validPoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            if (Vector3.Distance(player.position, point.position) <= spawnRadius &&
                Time.time - lastSpawnTime[point] >= pointCooldown)
            {
                validPoints.Add(point);
            }
        }

        if (validPoints.Count == 0)
        {
            Debug.Log("Không có spawn point nào khả dụng!");
            return;
        }

        // Chọn ngẫu nhiên spawn point
        Transform chosenPoint = validPoints[Random.Range(0, validPoints.Count)];
        lastSpawnTime[chosenPoint] = Time.time;

        // Chọn ngẫu nhiên enemy từ mảng prefab
        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Spawn enemy
        GameObject newEnemy = Instantiate(enemyToSpawn, chosenPoint.position, chosenPoint.rotation);
        activeEnemies.Add(newEnemy);

        Debug.Log($"Spawned {enemyToSpawn.name} tại {chosenPoint.position}");
    }
}
