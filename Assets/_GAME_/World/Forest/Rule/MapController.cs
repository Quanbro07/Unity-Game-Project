using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;

    private HashSet<Vector2Int> spawnedChunks = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, GameObject> chunkObjects = new Dictionary<Vector2Int, GameObject>();

    private const int chunkSize = 20;
    public float spawnCooldown = 0.2f;
    private float lastSpawnTime = 0f;

    public int loadRadius = 2;   // bán kính spawn chunk
    public int unloadRadius = 4; // bán kính xóa chunk (nên > loadRadius)

    private Vector2Int lastPlayerChunk = Vector2Int.zero;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void Start()
    {
        FindPlayer();
        if (player != null)
        {
            UpdateChunks();
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int currentChunk = WorldToChunkCoords(player.transform.position);
        if (currentChunk != lastPlayerChunk && Time.time - lastSpawnTime >= spawnCooldown)
        {
            lastPlayerChunk = currentChunk;
            lastSpawnTime = Time.time;
            UpdateChunks();
        }
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer;
                Debug.Log("MapController: Đã tìm thấy Player mới.");
            }
        }
    }

    Vector2Int WorldToChunkCoords(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / chunkSize);
        int y = Mathf.FloorToInt(worldPos.y / chunkSize);
        return new Vector2Int(x, y);
    }

    void UpdateChunks()
    {
        Vector2Int playerChunk = WorldToChunkCoords(player.transform.position);

        // Spawn chunk trong bán kính loadRadius
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                Vector2Int chunkCoords = playerChunk + new Vector2Int(x, y);
                if (!spawnedChunks.Contains(chunkCoords))
                {
                    SpawnChunk(chunkCoords);
                }
            }
        }

        // Xoá chunk chỉ khi quá xa unloadRadius
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in spawnedChunks)
        {
            if (Vector2Int.Distance(chunk, playerChunk) > unloadRadius)
            {
                chunksToRemove.Add(chunk);
            }
        }
        foreach (var chunk in chunksToRemove)
        {
            if (chunkObjects.ContainsKey(chunk))
            {
                Destroy(chunkObjects[chunk]);
                chunkObjects.Remove(chunk);
            }
            spawnedChunks.Remove(chunk);
        }
    }

    void SpawnChunk(Vector2Int chunkCoords)
    {
        Vector3 spawnPos = new Vector3(chunkCoords.x * chunkSize, chunkCoords.y * chunkSize, 0f);
        int randIndex = Random.Range(0, terrainChunks.Count);
        GameObject newChunk = Instantiate(terrainChunks[randIndex], spawnPos, Quaternion.identity);

        spawnedChunks.Add(chunkCoords);
        chunkObjects[chunkCoords] = newChunk;

        Debug.Log("Spawned chunk at: " + chunkCoords);
    }
}
