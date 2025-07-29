using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks; // Danh sách prefab map
    public GameObject player;              // Gán player trong Inspector
    public float checkerRadius = 0.2f;
    public LayerMask terrainMask;

    private PlayerController pm;
    private HashSet<Vector2Int> spawnedChunks = new HashSet<Vector2Int>();
    private const int chunkSize = 20;
    public float spawnCooldown = 0.2f; // thời gian giữa các lần spawn (giây)
private float lastSpawnTime = 0f;

    private Vector2Int lastPlayerChunk = Vector2Int.zero;

    void Start()
    {
        pm = FindObjectOfType<PlayerController>();

        if (player == null)
        {
            Debug.LogError("Player chưa được gán trong MapController!");
            return;
        }

        // Spawn chunk đầu tiên tại vị trí player
        Vector2Int playerChunk = WorldToChunkCoords(player.transform.position);
        lastPlayerChunk = playerChunk;
        SpawnChunk(playerChunk);

        // Spawn 8 chunk xung quanh để tránh bị "cản"
        SpawnNeighborChunks(playerChunk);
    }

    void Update()
    {
        if (player == null || pm == null)
        return;

    Vector2Int currentChunk = WorldToChunkCoords(player.transform.position);

    if (currentChunk != lastPlayerChunk && Time.time - lastSpawnTime >= spawnCooldown)
    {
        lastPlayerChunk = currentChunk;
        lastSpawnTime = Time.time;

        SpawnChunk(currentChunk);
        SpawnNeighborChunks(currentChunk);
    }
    }

    Vector2Int WorldToChunkCoords(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / chunkSize);
        int y = Mathf.FloorToInt(worldPos.y / chunkSize);
        return new Vector2Int(x, y);
    }

    void SpawnChunk(Vector2Int chunkCoords)
    {
        if (spawnedChunks.Contains(chunkCoords))
            return;

        Vector3 spawnPos = new Vector3(chunkCoords.x * chunkSize, chunkCoords.y * chunkSize, 0f);
        int randIndex = Random.Range(0, terrainChunks.Count);
        Instantiate(terrainChunks[randIndex], spawnPos, Quaternion.identity);
        spawnedChunks.Add(chunkCoords);

        Debug.Log("Spawned chunk at: " + chunkCoords);
    }

    void SpawnNeighborChunks(Vector2Int center)
    {
        // Xung quanh 8 hướng
        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = center + dir;
            SpawnChunk(neighbor);
        }
    }
}
