using System.Collections;
using UnityEngine; // Dòng này cực kỳ quan trọng, khắc phục lỗi CS02048

public class KnockBack : MonoBehaviour
{
    public bool IsKnocked { get; private set; } = false;

    public void ApplyKnockback(Rigidbody2D rb, Vector2 knockbackVector, float duration)
    {
        if (IsKnocked) return;
        StartCoroutine(PerformKnockback(rb, knockbackVector, duration));
    }

    private IEnumerator PerformKnockback(Rigidbody2D rb, Vector2 knockbackVector, float duration)
    {
        IsKnocked = true;
        rb.linearVelocity = knockbackVector; // Sửa lỗi "obsolete" tại đây
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        IsKnocked = false;
    }
}