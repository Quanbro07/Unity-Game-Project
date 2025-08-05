using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private KnockBack knockback;

    // Thêm các biến này để có thể tùy chỉnh lực và thời gian knockback của kẻ địch
    [SerializeField] private float enemyKnockbackForce = 10f;
    [SerializeField] private float enemyKnockbackDuration = 0.2f;

    private const int MAX_HEALTH = 100;

    // Sửa lỗi: Hàm TakeDamage cần nhận đủ 3 tham số để phù hợp với PlayerAttack và các script khác
    public void TakeDamage(int amount, Vector2 attackerPosition, Rigidbody2D targetRb)
    {
        health -= amount;

        // Sửa lỗi: Gọi hàm ApplyKnockback với đủ 3 tham số
        Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;
        knockback.ApplyKnockback(targetRb, knockbackDirection * enemyKnockbackForce, enemyKnockbackDuration);

        FlashOnHit();

        // Dead
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void FlashOnHit()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        if (animator != null)
            animator.enabled = false;

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.enabled = false; // Disappear
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;  // Appear
            yield return new WaitForSeconds(0.1f);
        }

        if (animator != null)
            animator.enabled = true;
    }
}