using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Health>() != null)
        {
            Health health = collision.GetComponent<Health>();
            health.TakeDamage(damage,transform.position);
        }
    }
}
