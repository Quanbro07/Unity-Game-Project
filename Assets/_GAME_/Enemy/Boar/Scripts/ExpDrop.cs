using UnityEngine;

public class ExpDrop : MonoBehaviour
{
    [Header("Experience Drop Settings")]
    public float orbSpawnSpread = 0.5f;

    [System.Serializable]
    public struct DropChance
    {
        public GameObject orbPrefab;
        public int minAmount;
        public int maxAmount;
        [Range(0f, 1f)] public float dropRate;
    }

    [SerializeField] private DropChance[] orbDropChances;

    /// <summary>
    /// Hàm Die: spawn orb + hủy object
    /// </summary>
    public void Die()
    {
        SpawnOrbs(transform.position);
        Debug.Log("Spawn Orb");
        Destroy(gameObject);
        Debug.Log($"{gameObject.name} died and dropped orbs!");
    }

    private void SpawnOrbs(Vector3 position)
    {
        foreach (DropChance drop in orbDropChances)
        {
            if (Random.value <= drop.dropRate)
            {
                int amountToDrop = Random.Range(drop.minAmount, drop.maxAmount + 1);

                for (int i = 0; i < amountToDrop; i++)
                {
                    if (drop.orbPrefab != null)
                    {
                        Vector3 spawnPosition = position + new Vector3(
                            Random.Range(-orbSpawnSpread, orbSpawnSpread),
                            Random.Range(-orbSpawnSpread, orbSpawnSpread),
                            0
                        );
                        Instantiate(drop.orbPrefab, spawnPosition, Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogWarning("⚠ Orb Prefab chưa được gán trong " + gameObject.name);
                    }
                }
            }
        }
    }
}

