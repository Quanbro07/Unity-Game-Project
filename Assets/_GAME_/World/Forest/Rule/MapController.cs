using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    [Header("Chunk Settings")]
    public List<GameObject> terrainChunks;
    public int chunkSize = 20;
    public int loadRadius = 2;
    public int unloadRadius = 4;
    public float spawnDelayPerChunk = 0.02f; // spawn mỗi chunk cách nhau 1 chút

    [Header("References")]
    public Transform player;

    private HashSet<Vector2Int> spawnedChunks = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, GameObject> chunkObjects = new Dictionary<Vector2Int, GameObject>();

    private Vector2Int lastPlayerChunk = Vector2Int.zero;
    private Queue<Vector2Int> chunksToSpawnQueue = new Queue<Vector2Int>();
    private List<GameObject> chunkPool = new List<GameObject>();

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
        UpdateChunksImmediate();
    }

    void Start()
    {
        FindPlayer();
        if (player != null)
        {
            UpdateChunksImmediate();
        }

        StartCoroutine(SpawnChunksGradually());
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int currentChunk = WorldToChunkCoords(player.position);
        if (currentChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentChunk;
            UpdateChunksImmediate();
        }
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }
    }

    Vector2Int WorldToChunkCoords(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / chunkSize);
        int y = Mathf.FloorToInt(worldPos.y / chunkSize);
        return new Vector2Int(x, y);
    }

    void UpdateChunksImmediate()
    {
        if (player == null) return;

        Vector2Int playerChunk = WorldToChunkCoords(player.position);

        // Xác định các chunk cần spawn
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                Vector2Int chunkCoords = playerChunk + new Vector2Int(x, y);
                if (!spawnedChunks.Contains(chunkCoords) && !chunksToSpawnQueue.Contains(chunkCoords))
                {
                    chunksToSpawnQueue.Enqueue(chunkCoords);
                }
            }
        }

        // Xóa chunk quá xa
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
            DespawnChunk(chunk);
        }
    }

    IEnumerator SpawnChunksGradually()
    {
        while (true)
        {
            if (chunksToSpawnQueue.Count > 0)
            {
                Vector2Int coords = chunksToSpawnQueue.Dequeue();
                SpawnChunk(coords);
                yield return new WaitForSeconds(spawnDelayPerChunk);
            }
            else
            {
                yield return null;
            }
        }
    }

    void SpawnChunk(Vector2Int chunkCoords)
    {
        GameObject chunk = GetChunkFromPool();
        chunk.transform.position = new Vector3(chunkCoords.x * chunkSize, chunkCoords.y * chunkSize, 0f);
        chunk.SetActive(true);

        spawnedChunks.Add(chunkCoords);
        chunkObjects[chunkCoords] = chunk;
    }

    void DespawnChunk(Vector2Int chunkCoords)
    {
        if (chunkObjects.ContainsKey(chunkCoords))
        {
            GameObject chunk = chunkObjects[chunkCoords];
            chunk.SetActive(false);
            chunkPool.Add(chunk);

            chunkObjects.Remove(chunkCoords);
            spawnedChunks.Remove(chunkCoords);
        }
    }

    GameObject GetChunkFromPool()
    {
        if (chunkPool.Count > 0)
        {
            GameObject pooledChunk = chunkPool[0];
            chunkPool.RemoveAt(0);
            return pooledChunk;
        }
        else
        {
            int randIndex = Random.Range(0, terrainChunks.Count);
            return Instantiate(terrainChunks[randIndex]);
        }
    }
}
