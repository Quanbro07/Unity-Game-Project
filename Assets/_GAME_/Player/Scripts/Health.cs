using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private GameObject player; 
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private KnockBack knockback;

    private const int MAX_HEALTH = 100;
        
    public void TakeDamage(int amount,Vector2 attackerPosition)
    {
        health -= amount;
        knockback.ApplyKnockback(attackerPosition);

        if (player.CompareTag("Player"))
        {
            PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.ForceCancelAttack(); // Ngắt attack + combo + slash
            }

            // Flash & disable effects nếu cần
            FlashOnHit();
        }


        // Dead
        if (health <= 0)
        {
            Debug.Log("Enemy Destroyed");
            Destroy(gameObject);
        }
    }


    public void FlashOnHit()
    {
        Debug.Log("FLASH");
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        if (animator != null)
            animator.enabled = false;

        // Tắt slash hoặc effect liên quan ngay từ đầu
        PlayerAnimationEvents playerEvents = player.GetComponent<PlayerAnimationEvents>();
        if (playerEvents != null)
            playerEvents.DisableAll(); // ← Tắt slash ngay

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
