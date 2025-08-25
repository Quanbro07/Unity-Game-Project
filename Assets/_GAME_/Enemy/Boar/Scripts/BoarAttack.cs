using UnityEngine;

public class BoarAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

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