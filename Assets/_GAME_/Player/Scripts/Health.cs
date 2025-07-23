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
            Debug.Log("PLAYER");
            
            PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                Debug.Log("Disable attack input");
                playerAttack.DisableAttackTemporarily(0.5f); // Khóa trong 0.5 giây
            }

            PlayerAnimationEvents playerAnimationEvents = player.GetComponent<PlayerAnimationEvents>();
            if (playerAnimationEvents != null)
            {
                Debug.Log("DISABLEEEE");
                playerAnimationEvents.DisableAll();
            }

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
