using Pathfinding; // Đảm bảo bạn đã import A* Pathfinding Project vào project của mình
using System.Collections;
using UnityEngine;

public class StickySnailController : MonoBehaviour
{
    // Các biến A* Pathfinding và Renderer
    public AIPath aiPath;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AIDestinationSetter destinationSetter;

    #region Animation
    [SerializeField] private Animator animator;
    private int snailIdle = Animator.StringToHash("SnailIdle");
    private int snailRun = Animator.StringToHash("SnailRun");
    private int currentAnimation;
    #endregion

    #region States
    public enum State { Wander, Flee, GoToFleePos }
    private State currentState = State.Wander;
    #endregion

    #region RandomWalk
    [SerializeField] private float wanderRadius = 3f;
    private Transform wanderTarget;
    private float wanderCooldown = 3f;
    [SerializeField] private float idleDelay = 2f; // Thời gian ốc sên chờ trước khi di chuyển tiếp
    private bool isWaitingToWander = false; // Biến cờ để kiểm soát trạng thái chờ
    #endregion

    #region Flee
    [SerializeField] private Transform player; // Kéo Player Transform vào đây trong Inspector
    [SerializeField] private float fleeDistance = 5f; // Khoảng cách tối thiểu ốc sên muốn giữ với người chơi
    private Transform fleeTarget; // Vị trí mà ốc sên sẽ chạy tới khi Flee
    private bool isFleeing = false; // Biến cờ cho biết đang ở trạng thái Flee hay không
    #endregion

    #region Health System
    [Header("Health System")]
    public float maxHealth = 15f; // Máu tối đa của Sticky Snail
    private float currentHealth; // Máu hiện tại
    #endregion

    #region Experience Drop
    [Header("Experience Drop")]
    public float orbSpawnSpread = 0.5f; // Khoảng cách ngẫu nhiên các orb rơi ra từ điểm chết của ốc sên

    // MẢNG CÁC PREFAB EXP ORB ĐỂ RƠI RA TRỰC TIẾP KHI QUÁI VẬT CHẾT
    [SerializeField] private GameObject[] experienceOrbPrefabsToDrop; // Mảng các Prefab EXP Orb
    [SerializeField] private int[] experienceOrbDropCounts;

    [Header("Exp Orb Value Range")]
    [SerializeField] private int minExpValuePerOrb = 1; // Giá trị EXP tối thiểu cho mỗi viên orb được sinh ra
    [SerializeField] private int maxExpValuePerOrb = 100; // Giá trị EXP tối đa cho mỗi viên orb được sinh ra
    #endregion

    private void Start()
    {
        // Khởi tạo các GameObject tạm thời để làm mục tiêu cho A* Pathfinding
        wanderTarget = new GameObject("SnailWanderTarget").transform;
        fleeTarget = new GameObject("SnailFleeTarget").transform;

        // Khởi tạo máu ban đầu
        currentHealth = maxHealth;

        // Cố gắng tìm Player nếu chưa được gán trong Inspector (hữu ích cho việc debug)
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found! Please assign Player Transform to StickySnailController or ensure Player has 'Player' tag.");
            }
        }

        // Đặt trạng thái ban đầu là Wander
        SetState(State.Wander);
    }

    private void Update()
    {
        // Xử lý logic tương ứng với trạng thái hiện tại của ốc sên
        if (currentState == State.Wander)
        {
            WanderLogic();
        }
        else if (currentState == State.Flee)
        {
            FleeLogic();
        }
        else if (currentState == State.GoToFleePos)
        {
            CheckReachedFleePos();
        }

        // Cập nhật hướng di chuyển và animation của ốc sên
        UpdateDirection();
        UpdateAnimation(currentAnimation);
    }

    private void UpdateDirection()
    {
        // Lấy vận tốc mong muốn từ A* Pathfinding để xác định hướng
        Vector2 velocity = aiPath.desiredVelocity;

        // Nếu vận tốc quá nhỏ, ốc sên đang đứng yên, chuyển sang animation Idle
        if (velocity.sqrMagnitude < 0.01f)
        {
            currentAnimation = snailIdle;
            return;
        }

        // Nếu đang di chuyển, chuyển sang animation Run
        currentAnimation = snailRun;
        // Lật sprite theo chiều X nếu ốc sên di chuyển sang trái
        spriteRenderer.flipX = velocity.x < 0;
    }

    private void UpdateAnimation(int animation)
    {
        // Chuyển đổi animation một cách mượt mà
        animator.CrossFade(animation, 0);
    }

    public void SetState(State newState)
    {
        currentState = newState;

        if (newState == State.Wander)
        {
            isFleeing = false; // Không còn trong trạng thái chạy trốn
            StopAllCoroutines(); // Dừng tất cả các coroutine đang chạy (ví dụ: DelayBeforeWander)
            isWaitingToWander = false; // Reset cờ chờ

            // Vô hiệu hóa pathfinding ban đầu để tạo độ trễ trước khi Wander
            aiPath.enabled = false;
            destinationSetter.enabled = false;

            // Bắt đầu coroutine chờ trước khi Wander
            if (!isWaitingToWander)
            {
                StartCoroutine(DelayBeforeWander());
            }
        }
        else if (newState == State.Flee)
        {
            isFleeing = true; // Đang chạy trốn
            StopAllCoroutines(); // Dừng mọi coroutine khác
            isWaitingToWander = false;

            // Kích hoạt pathfinding và đặt mục tiêu là fleeTarget
            aiPath.enabled = true;
            destinationSetter.enabled = true;
            destinationSetter.target = fleeTarget;
        }
        else if (newState == State.GoToFleePos)
        {
            isFleeing = false; // Đã dừng chạy trốn nhưng vẫn tiếp tục đi về vị trí cuối cùng
            StopAllCoroutines();
            isWaitingToWander = false;

            aiPath.enabled = true;
            destinationSetter.enabled = true;
            // Mục tiêu vẫn là fleeTarget đã được tính toán trong FleeLogic cuối cùng
        }
    }

    #region Wander Logic
    private void WanderLogic()
    {
        // Nếu đang trong trạng thái chờ, không làm gì
        if (isWaitingToWander) return;

        // Nếu pathfinding chưa được bật, bật nó lên
        if (!aiPath.enabled)
        {
            aiPath.enabled = true;
            destinationSetter.enabled = true;
        }

        // Giảm thời gian chờ wander
        wanderCooldown -= Time.deltaTime;
        // Nếu hết thời gian chờ hoặc đã đến cuối đường dẫn, đặt mục tiêu wander mới
        if (wanderCooldown <= 0 || aiPath.reachedEndOfPath || aiPath.remainingDistance < 0.2f)
        {
            SetRandomWanderTarget();
            wanderCooldown = wanderCooldown; // Reset thời gian chờ
        }
    }

    private void SetRandomWanderTarget()
    {
        // Đặt một điểm ngẫu nhiên trong bán kính wanderRadius làm mục tiêu
        Vector2 origin = transform.position;
        Vector2 randomPoint = origin + Random.insideUnitCircle * wanderRadius;
        wanderTarget.position = randomPoint;
        destinationSetter.target = wanderTarget;
    }

    private IEnumerator DelayBeforeWander()
    {
        isWaitingToWander = true; // Đặt cờ đang chờ
        yield return new WaitForSeconds(idleDelay); // Chờ một khoảng thời gian

        // Nếu trạng thái vẫn là Wander sau khi chờ, bắt đầu di chuyển
        if (currentState == State.Wander)
        {
            isWaitingToWander = false; // Bỏ cờ chờ
            SetRandomWanderTarget(); // Đặt mục tiêu wander đầu tiên
            aiPath.enabled = true; // Bật pathfinding
            destinationSetter.enabled = true;
        }
    }
    #endregion

    #region Flee Logic
    private void FleeLogic()
    {
        if (player != null)
        {
            // Tính toán hướng chạy trốn (ngược lại với hướng đến người chơi)
            Vector3 dirAway = (transform.position - player.position).normalized;
            // Đặt vị trí mục tiêu chạy trốn xa player theo fleeDistance
            Vector3 targetPos = transform.position + dirAway * fleeDistance;
            fleeTarget.position = targetPos; // Cập nhật vị trí mục tiêu
            // AIPath sẽ tự động di chuyển đến fleeTarget
        }
    }

    public void StartFleeing()
    {
        // Chuyển trạng thái sang Flee
        SetState(State.Flee);
    }

    public void StopFleeing()
    {
        // Khi người chơi không còn trong tầm, chuyển sang trạng thái GoToFleePos để đi về vị trí cuối cùng
        SetState(State.GoToFleePos);
    }

    private void CheckReachedFleePos()
    {
        
            // Kiểm tra xem ốc sên đã đến vị trí chạy trốn cuối cùng chưa
            if (aiPath.reachedEndOfPath || aiPath.remainingDistance < 0.5f)
            {
                // Nếu đã đến, chuyển về trạng thái Wander để bắt đầu chu trình nghỉ và di chuyển ngẫu nhiên
                SetState(State.Wander);
            }
  
    }
    #endregion

    // =========================================================
    // HÀM NHẬN SÁT THƯƠNG VÀ CHẾT
    // =========================================================

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Giảm máu
        Debug.Log(gameObject.name + " took " + damage + " damage. Current Health: " + currentHealth);

        // Nếu máu hết, ốc sên chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Duyệt qua mảng các Prefab Orb
        // Đảm bảo rằng cả hai mảng có cùng kích thước để tránh lỗi Index Out Of Range
        if (experienceOrbPrefabsToDrop.Length != experienceOrbDropCounts.Length)
        {
            Debug.LogError("Experience Orb Prefabs and Drop Counts arrays do not have the same length! Please check the Inspector for " + gameObject.name);
            // Có thể muốn return ở đây hoặc xử lý lỗi khác
            Destroy(gameObject); // Vẫn hủy đối tượng để tránh vòng lặp vô hạn
            return;
        }

        for (int i = 0; i < experienceOrbPrefabsToDrop.Length; i++)
        {
            GameObject orbPrefab = experienceOrbPrefabsToDrop[i];
            int dropCount = experienceOrbDropCounts[i];

            if (orbPrefab != null)
            {
                for (int j = 0; j < dropCount; j++) // Lặp lại 'dropCount' lần
                {
                    // Tính toán vị trí sinh ra Orb ngẫu nhiên xung quanh vị trí chết của ốc sên
                    Vector3 spawnPosition = transform.position + new Vector3(
                        Random.Range(-orbSpawnSpread, orbSpawnSpread),
                        Random.Range(-orbSpawnSpread, orbSpawnSpread),
                        0
                    );
                    // Sinh ra viên EXP Orb
                    GameObject spawnedOrb = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);

                    // Lấy script ExperienceOrb từ viên EXP vừa sinh ra
                    ExperienceOrb orbScript = spawnedOrb.GetComponent<ExperienceOrb>();
                    if (orbScript != null)
                    {
                        // Gán giá trị EXP ngẫu nhiên cho viên Orb trong phạm vi đã định
                        orbScript.experienceAmount = Random.Range(minExpValuePerOrb, maxExpValuePerOrb + 1);
                    }
                    else
                    {
                        Debug.LogError("Spawned Experience Orb Prefab (" + orbPrefab.name + ") is missing ExperienceOrb script! Please ensure your orb prefabs have this script.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("One of the Experience Orb Prefabs at index " + i + " in the array is not assigned on " + gameObject.name + "! Please assign all desired orb prefabs in the Inspector.");
            }
        }

        // Hủy đối tượng ốc sên sau khi chết và rơi orb
        Destroy(gameObject);
        Debug.Log(gameObject.name + " died and dropped experience orbs.");
    }

    // Hàm OnMouseDown() chỉ để test nhanh trong Unity Editor, không dùng trong game thực tế
    
}