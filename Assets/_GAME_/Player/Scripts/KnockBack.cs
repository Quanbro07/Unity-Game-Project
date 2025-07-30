using System.Collections;
using UnityEngine;
using Pathfinding;

public class KnockBack : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    
    public bool IsKnocked => isKnocked;
    
    private bool isKnocked = false;

    private AIPath aiPath;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>(); // Lấy AIPath khi start
    }

    public void ApplyKnockback(Vector2 sourcePosition)
    {
        if (rb == null || isKnocked)
        {
            return;
        }

        Vector2 direction = (rb.position - (Vector2)sourcePosition).normalized;

        if (direction.x < 0)
        {
            spriteRender.flipX = false;
        }
        else if(direction.x > 0)
        {
            spriteRender.flipX = true;
        }

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        if (aiPath != null)
        {
            aiPath.canMove = false;
        }

        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        rb.linearVelocity = Vector2.zero;

        if (aiPath != null)
            aiPath.canMove = true;
    }
}
