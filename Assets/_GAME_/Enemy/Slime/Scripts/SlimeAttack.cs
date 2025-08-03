// Đây là ví dụ về cách bạn nên sửa SlimeAttack.cs
// Hãy tìm hàm gây sát thương trong script này và thay đổi nó
using UnityEngine;

public class SlimeAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1; // Giả định
    [SerializeField] private float knockbackForce = 5f; // Giả định
    [SerializeField] private float knockbackDuration = 0.2f; // Giả định

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerHealth != null && playerRb != null)
            {
                // Gọi hàm TakeDamage với đủ 3 tham số
                playerHealth.TakeDamage(damageAmount, transform.position, playerRb);
            }
        }
    }
}