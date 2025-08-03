using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Tốc độ di chuyển của người chơi

    void Update()
    {
        // Lấy input từ bàn phím
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D hoặc Left/Right Arrow
        float moveY = Input.GetAxisRaw("Vertical");   // W/S hoặc Up/Down Arrow

        // Tạo vector di chuyển
        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized; // .normalized để di chuyển chéo không nhanh hơn

        // Di chuyển người chơi
        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    // Tạm thời thêm hàm TakeDamage vào đây để test.
    // Sau này bạn sẽ có một script quản lý máu riêng cho Player.
    public void TakeDamage(float damage)
    {
        // Ví dụ: gọi GameManager để giảm HP của người chơi
        if (GameManagerController.Instance != null)
        {
            GameManagerController.Instance.TakeDamage(damage);
        }
        Debug.Log("Player took " + damage + " damage!");
    }
}