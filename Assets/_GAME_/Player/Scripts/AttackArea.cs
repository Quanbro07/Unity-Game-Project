using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 3;
    private HashSet<Collider2D> damagedTargets = new HashSet<Collider2D>();

    // Tham chiếu đến Rigidbody2D của người chơi để truyền vào hàm TakeDamage
    private Rigidbody2D playerRb;

    private void Awake()
    {
        // Lấy Rigidbody2D từ đối tượng cha (Player)
        playerRb = GetComponentInParent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Khi bắt đầu 1 lần tấn công mới thì reset danh sách đã trúng
        damagedTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        // Kiểm tra xem đối tượng có Health component và chưa bị tấn công trong lần này không
        if (health != null && !damagedTargets.Contains(collision))
        {
            damagedTargets.Add(collision); // Đánh dấu đã trúng

            // Lấy Rigidbody2D của đối tượng bị tấn công
            Rigidbody2D targetRb = collision.GetComponent<Rigidbody2D>();

            if (targetRb != null)
            {
                // Sửa lỗi: Truyền đủ 3 tham số
                health.TakeDamage(damage, transform.position, targetRb);
            }
        }
    }
}