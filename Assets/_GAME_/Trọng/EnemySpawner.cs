using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Kéo Prefab của heo và sên vào 2 ô trống này trong Inspector
    [SerializeField] private GameObject boarPrefab;
    [SerializeField] private GameObject stickySnailPrefab;

    // Thời gian giữa các lần tạo quái
    [SerializeField] private float spawnInterval = 5f;

    // Tạo các đối tượng trống trong Hierarchy rồi kéo vào mảng này
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        // Gọi hàm SpawnEnemy lần đầu tiên sau 0 giây, và lặp lại sau mỗi spawnInterval
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Đảm bảo mảng spawnPoints không rỗng
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points array is not assigned or is empty!");
            return;
        }

        // Chọn ngẫu nhiên một vị trí trong mảng spawnPoints
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnPointIndex];

        // Chọn ngẫu nhiên loại kẻ địch để tạo
        GameObject enemyToSpawn;
        if (Random.value > 0.5f) // Ví dụ: 50% là heo, 50% là sên
        {
            enemyToSpawn = boarPrefab;
        }
        else
        {
            enemyToSpawn = stickySnailPrefab;
        }

        // Tạo ra kẻ địch tại vị trí và góc quay của spawnPoint đã chọn
        Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Spawning enemy: " + enemyToSpawn.name);
    }
}