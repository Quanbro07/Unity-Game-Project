using UnityEngine;
using Pathfinding;   // Quan trọng: để dùng AstarPath

public class ChunkSpawner : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public int chunkSize = 16;

    public GameObject SpawnChunk(Vector2Int coords)
    {
        int randIndex = Random.Range(0, chunkPrefabs.Length);
        GameObject chunk = Instantiate(chunkPrefabs[randIndex]);

        chunk.transform.position = new Vector3(coords.x * chunkSize, coords.y * chunkSize, 0f);

        // Lấy bounds của chunk để update graph
        Bounds bounds = chunk.GetComponent<Renderer>() != null
            ? chunk.GetComponent<Renderer>().bounds
            : new Bounds(chunk.transform.position, new Vector3(chunkSize, chunkSize, 0));

        // Báo A* cập nhật vùng này
        AstarPath.active.UpdateGraphs(bounds);

        return chunk;
    }
}