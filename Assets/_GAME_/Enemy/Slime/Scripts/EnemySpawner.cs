using Pathfinding;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private Vector3 spawnPosition;

    public void SpawnEnemy(Vector3 spawnPosition)
    {
        StartCoroutine(delaySpawn(spawnPosition));
    }
    
    private void Spawn(Vector3 spawnPosition)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Gán AI target sau khi spawn
        AIPath aiPath = enemy.GetComponent<AIPath>();
        if (aiPath != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                aiPath.destination = player.transform.position; // hoặc gán target nếu có
                aiPath.canMove = true;

                // Nếu dùng AIDestinationSetter
                AIDestinationSetter setter = enemy.GetComponent<AIDestinationSetter>();
                if (setter != null)
                    setter.target = player.transform;
            }
        }
    }


    private IEnumerator delaySpawn(Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(0.25f);
        Spawn(spawnPosition);
    }


    public void setSpawnPoint(Transform deadTransform)
    {
        this.spawnPosition = deadTransform.position; // Lưu tọa độ, không phải Transform
    }
}
