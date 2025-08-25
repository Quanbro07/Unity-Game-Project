using UnityEngine;

public class EnemySimple : MonoBehaviour
{
    public float maxHealth = 10f; // Máu tối đa của quái vật
    private float currentHealth;

    // Kéo Prefab ExperienceOrb vào đây trong Inspector
    public GameObject experienceOrbPrefab;
    public int numberOfOrbsToDrop = 1; // Số lượng viên kinh nghiệm sẽ rơi ra

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm để nhận sát thương
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hàm xử lý khi quái vật chết
    void Die()
    {
        // Sinh ra các viên kinh nghiệm
        for (int i = 0; i < numberOfOrbsToDrop; i++)
        {
            // Vị trí sinh ra viên kinh nghiệm (tại vị trí của quái vật, thêm một chút ngẫu nhiên)
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            Instantiate(experienceOrbPrefab, spawnPosition, Quaternion.identity);
        }

        // Hủy đối tượng quái vật
        Destroy(gameObject);
        Debug.Log(gameObject.name + " died and dropped " + numberOfOrbsToDrop + " experience orb(s).");
    }

    // (Tùy chọn) Thêm cách để gây sát thương cho quái vật (ví dụ: click chuột để test)
    void OnMouseDown() // Hàm này được gọi khi click chuột vào collider 2D của Enemy
    {
        TakeDamage(2f); // Gây 2 sát thương mỗi lần click để test
    }
}