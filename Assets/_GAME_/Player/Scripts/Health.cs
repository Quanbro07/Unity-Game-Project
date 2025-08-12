using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    // ... các biến khác của bạn ...
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private KnockBack knockback;
    [SerializeField] private BoarController boarController;
    // Thêm tham chiếu đến PlayerController
    [SerializeField] private PlayerController playerController;
    
    private const int MAX_HEALTH = 100;
    [SerializeField] private int health = 5; // Đặt giá trị health ban đầu

    public void TakeDamage(int amount, Vector2 attackerPosition, Rigidbody2D targetRb)
    {
        health -= amount;

        Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;
        knockback.ApplyKnockback(targetRb, knockbackDirection * 10f, 0.2f); // Sử dụng giá trị cố định hoặc biến của bạn

        FlashOnHit();

        if (health <= 0)
        {
            if (playerController != null)
            {
                // Gọi hàm PlayerDied() trong PlayerController
                playerController.PlayerDied();
            }
            else
            {
                boarController.Die();
                              // Nếu không phải là Player, thì Destroy đối tượng
                Destroy(gameObject);
            }
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
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        if (animator != null)
            animator.enabled = true;
    }
}