using System.Collections;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    
    public bool IsKnocked => isKnocked;
    
    private bool isKnocked = false;

    public void ApplyKnockback(Vector2 sourcePosition)
    {
        if (rb == null || isKnocked) return;

        Vector2 direction = (rb.position - (Vector2)sourcePosition).normalized;
        Debug.Log($"Knockback Direction: {direction}, Force: {direction * knockbackForce}");
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

        StartCoroutine(KnockbackRoutine());
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        rb.linearVelocity = Vector2.zero;
    }
}
