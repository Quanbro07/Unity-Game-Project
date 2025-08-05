using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    [Header("Experience Orb Settings")]
    public int experienceAmount = 1; // Giá trị kinh nghiệm mặc định cho orb này

    [SerializeField] private float collectRadius = 0.5f; // Bán kính để người chơi có thể nhặt orb
    [SerializeField] private float attractSpeed = 5f; // Tốc độ orb bay về phía người chơi

    private Transform playerTransform; // Tham chiếu đến vị trí người chơi

    void Start()
    {
        // Tùy chọn: Tìm Player khi bắt đầu nếu chưa được gán
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        // Logic hút orb về phía người chơi khi đủ gần
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Nếu người chơi đủ gần, bắt đầu hút orb
            if (distanceToPlayer <= collectRadius)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, attractSpeed * Time.deltaTime);
            }
        }
    }


    // Hàm này sẽ được gọi khi một Collider 2D khác đi vào trigger của orb
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (other.CompareTag("Player"))
        {
            // Lấy script PlayerController từ Player
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Cộng kinh nghiệm cho Player
                player.AddExperience(experienceAmount);

                // Hủy viên orb sau khi đã được nhặt
                Destroy(gameObject);
            }
        }
    }

    // Tùy chọn: Để dễ nhìn trong Editor khi debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}