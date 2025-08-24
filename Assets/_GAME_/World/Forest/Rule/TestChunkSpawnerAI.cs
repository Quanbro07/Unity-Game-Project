using UnityEngine;

public class TestChunkSpawnerAI : MonoBehaviour
{
    public ChunkGraphSpawner spawner;

    void Start()
    {
        spawner.SpawnChunk(new Vector2Int(0, 0));
        spawner.SpawnChunk(new Vector2Int(1, 0));
    }
}
