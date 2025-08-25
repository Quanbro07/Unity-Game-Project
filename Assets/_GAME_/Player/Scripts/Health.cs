using System.Collections;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public event Action<int, int> OnHealthChanged;

    // ... các biến khác của bạn ...
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private KnockBack knockback;
    [SerializeField] private ExpDrop expDrop;
    // Thêm tham chiếu đến PlayerController
    [SerializeField] private PlayerController playerController;
    
    private const int MAX_HEALTH = 100;
    [SerializeField] private int health = 5; // Đặt giá trị health ban đầu
    private bool isDead = false;

    private void Awake()
    {
        expDrop = GetComponent<ExpDrop>();
        OnHealthChanged?.Invoke(health, MAX_HEALTH); // báo UI ngay khi start
    }

    public void TakeDamage(int amount, Vector2 attackerPosition, Rigidbody2D targetRb)
    {
        if (isDead) return;

        health -= amount;
        OnHealthChanged?.Invoke(health, MAX_HEALTH);

        Vector2 knockbackDirection = ((Vector2)transform.position - attackerPosition).normalized;
        knockback.ApplyKnockback(targetRb, knockbackDirection * 10f, 0.2f); // Sử dụng giá trị cố định hoặc biến của bạn

        FlashOnHit();

        if (health <= 0)
        {
            isDead = true;

            if (playerController != null)
            {
                // Gọi hàm PlayerDied() trong PlayerController
                playerController.PlayerDied();
            }
            else
            {
                if (gameObject.CompareTag("BigSlime"))
                {
                    GameObject managerObj = GameObject.Find("EnemyManager");
                    if (managerObj != null)
                    {
                        SlimeSpawner slimeSpawner = managerObj.GetComponent<SlimeSpawner>();
                        if (slimeSpawner != null)
                        {
                            Vector3 deadPoint = transform.position;
                            Vector3 spawnPoint1 = deadPoint - new Vector3(1, 1, 0);
                            Vector3 spawnPoint2 = deadPoint + new Vector3(1, 1, 0);

                            slimeSpawner.SpawnEnemy(spawnPoint1);
                            slimeSpawner.SpawnEnemy(spawnPoint2);
                        }
                    }
                }

                expDrop.Die();
                              // Nếu không phải là Player, thì Destroy đối tượng
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