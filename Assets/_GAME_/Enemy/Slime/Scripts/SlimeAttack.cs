using UnityEngine;

public class SlimeAttack : MonoBehaviour
{
    [SerializeField] private int damage = 3;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage, transform.position);
        }
    }
}
