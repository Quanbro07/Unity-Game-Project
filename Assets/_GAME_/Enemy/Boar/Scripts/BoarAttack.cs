using UnityEngine;

public class BoarAttack : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            KnockBack playerKnockBack = collision.gameObject.GetComponent<KnockBack>();

            if (playerController != null && playerRb != null && playerKnockBack != null)
            {
                playerController.TakeDamage(damage);

                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                playerKnockBack.ApplyKnockback(playerRb, knockbackDirection * knockbackForce, knockbackDuration);
            }
        }
    }
}