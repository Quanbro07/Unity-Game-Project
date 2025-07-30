using UnityEngine;

public class BoarAttack : MonoBehaviour
{
    [SerializeField] private int damage = 3;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, transform.position);
            }
        }
    }
}
