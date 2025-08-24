using UnityEngine;
using Pathfinding;

public class ChunkGraphSpawner : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public int chunkSize = 16;
    public LayerMask obstacleLayer = -1; // Layer của obstacle

    public GameObject SpawnChunk(Vector2Int coords)
    {
        // Spawn chunk prefab
        int randIndex = Random.Range(0, chunkPrefabs.Length);
        GameObject chunk = Instantiate(chunkPrefabs[randIndex]);
        chunk.transform.position = new Vector3(coords.x * chunkSize, coords.y * chunkSize, 0f);

        // Update A* pathfinding graph cho chunk này
        UpdateChunkPathfinding(coords, chunk);

        return chunk;
    }

    public void UpdateChunkPathfinding(Vector2Int coords, GameObject chunk)
    {
        if (AstarPath.active == null) return;

        // Tính bounds chính xác cho chunk
        Bounds bounds = new Bounds(
            new Vector3(coords.x * chunkSize + chunkSize / 2f, coords.y * chunkSize + chunkSize / 2f, 0f),
            new Vector3(chunkSize, chunkSize, 1f)
        );

        // Tạo GraphUpdateObject
        GraphUpdateObject guo = new GraphUpdateObject(bounds);
        guo.updatePhysics = true; // Sử dụng physics để detect obstacle
        guo.modifyWalkability = true;

        // Không cần set setWalkability = false vì updatePhysics sẽ tự động detect

        // Apply update - chỉ update local area thôi
        AstarPath.active.UpdateGraphs(guo);

        // KHÔNG dùng AstarPath.active.Scan() vì nó sẽ rescan toàn bộ map (chậm)

        Debug.Log($"Updated pathfinding for chunk at {coords}");
    }

    public void ClearChunkPathfinding(Vector2Int coords)
    {
        if (AstarPath.active == null) return;

        Bounds bounds = new Bounds(
            new Vector3(coords.x * chunkSize + chunkSize / 2f, coords.y * chunkSize + chunkSize / 2f, 0f),
            new Vector3(chunkSize, chunkSize, 1f)
        );

        // Clear area (set về walkable)
        GraphUpdateObject guo = new GraphUpdateObject(bounds);
        guo.updatePhysics = true;
        guo.modifyWalkability = true;
        guo.setWalkability = true; // Force set về walkable

        AstarPath.active.UpdateGraphs(guo);

        Debug.Log($"Cleared pathfinding for chunk at {coords}");
    }
}