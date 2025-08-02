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

            if(gameObject.CompareTag("BigSlime"))
            {
                GameObject managerObj = GameObject.Find("EnemyManager");
                if (managerObj != null)
                {
                    EnemySpawner enemySpawner = managerObj.GetComponent<EnemySpawner>();
                    if (enemySpawner != null)
                    {
                        Vector3 deadPoint = transform.position;
                        Vector3 spawnPoint1 = deadPoint - new Vector3(1, 1, 0);
                        Vector3 spawnPoint2 = deadPoint + new Vector3(1, 1, 0);

                        enemySpawner.SpawnEnemy(spawnPoint1);
                        enemySpawner.SpawnEnemy(spawnPoint2);
                    }
                }
            }

            Destroy(gameObject);

        }
    }

    public void TakePoisionDamage(int amount)
    {
        health -= amount;

        if (player.CompareTag("Player"))
        {
            PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.ForceCancelAttack(); // Ngắt attack + combo + slash
            }

            // Flash & disable effects nếu cần
            FlashPoisonEffect();
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
        StartCoroutine(FlashCoroutine());
    }
    private void FlashPoisonEffect()
    {
        // Ví dụ đơn giản: đổi màu xanh lá trong 0.1s
        StartCoroutine(FlashColor(Color.green, 0.3f));
    }

    private IEnumerator FlashColor(Color flashColor, float duration)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogWarning("No SpriteRenderer found on Player or its children.");
            yield break;
        }

        Color originalColor = sr.color;
        sr.color = flashColor;
        
        FlashOnHit();
        yield return new WaitForSeconds(duration);

        sr.color = originalColor;
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
