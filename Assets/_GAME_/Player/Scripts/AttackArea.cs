using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 3;
    private HashSet<Collider2D> damagedTargets = new HashSet<Collider2D>();

    private void OnEnable()
    {
        // Khi bắt đầu 1 lần tấn công mới thì reset danh sách đã trúng
        damagedTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Health>() != null && !damagedTargets.Contains(collision))
        {
            damagedTargets.Add(collision); // Đánh dấu đã trúng
            Health health = collision.GetComponent<Health>();
            health.TakeDamage(damage, transform.position);
        }
    }
}
