using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour // Tên class đã được đổi và PHẢI khớp với tên file
{
    // === BIẾN UI (Cần gán trong Inspector) ===
    public GameObject pauseMenuUI; // Kéo PauseBackgroundPanel vào đây (hoặc PauseMenuPanel)
    public Image hpBarFillImage;   // Kéo ExpBarFill của HP vào đây
    public Image expBarFillImage;  // Kéo ExpBarFill của EXP vào đây

    // === BIẾN THỐNG KÊ NGƯỜI CHƠI ===
    [Header("Player Stats")] // Tiêu đề trong Inspector để dễ nhìn
    public float maxHp = 100f;
    private float currentHp;
    public float maxExperienceForNextLevel = 100f; // EXP cần để lên cấp tiếp theo
    private float currentExperience = 0f;
    public int currentLevel = 1;

    // === BIẾN KHÁC ===
    private bool isPaused = false;

    // === SINGLETON PATTERN ===
    // Cho phép các script khác truy cập dễ dàng vào GameManager này
    public static GameManagerController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Nếu bạn muốn GameManager tồn tại qua các scene, uncomment dòng dưới:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ có một GameManager tồn tại
        }
    }

    void Start()
    {
        // Khởi tạo HP và EXP khi bắt đầu game
        currentHp = maxHp;
        UpdateHpBar(); // Cập nhật thanh HP ban đầu

        currentExperience = 0; // Đặt EXP về 0 khi bắt đầu scene
        UpdateExpBar(); // Cập nhật thanh EXP ban đầu

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Đảm bảo menu bị ẩn khi game bắt đầu
        }
        Time.timeScale = 1f; // Đảm bảo thời gian game chạy bình thường
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !isPaused)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game Restarted!");
    }

    public void QuitGame()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    // === HÀM HP ===
    public void UpdateHpBar()
    {
        if (hpBarFillImage != null)
        {
            hpBarFillImage.fillAmount = currentHp / maxHp;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0); // Đảm bảo HP không âm
        UpdateHpBar();

        if (currentHp <= 0)
        {
            Debug.Log("Player Died!");
            // TODO: Thêm logic chết người chơi ở đây (ví dụ: Show Game Over Screen, Restart Game)
        }
    }

    // === HÀM EXP ===
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log("Added " + amount + " EXP. Current EXP: " + currentExperience + " / " + maxExperienceForNextLevel);

        // Kiểm tra xem đã đủ EXP để lên cấp chưa
        while (currentExperience >= maxExperienceForNextLevel)
        {
            currentExperience -= maxExperienceForNextLevel; // Trừ đi lượng EXP cần thiết cho cấp hiện tại
            currentLevel++; // Tăng cấp
            maxExperienceForNextLevel *= 1.2f; // Tăng lượng EXP cần thiết cho cấp tiếp theo (có thể điều chỉnh công thức này)
            Debug.Log("Leveled Up! New Level: " + currentLevel + ", Next Level EXP Required: " + maxExperienceForNextLevel);
            // TODO: Thêm hiệu ứng lên cấp, hồi máu, tăng chỉ số... ở đây
        }
        UpdateExpBar(); // Cập nhật thanh EXP sau khi thêm EXP
    }

    public void UpdateExpBar()
    {
        if (expBarFillImage != null)
        {
            // Đảm bảo không chia cho 0 nếu maxExperienceForNextLevel = 0
            expBarFillImage.fillAmount = (maxExperienceForNextLevel > 0) ? (currentExperience / maxExperienceForNextLevel) : 0;
        }
    }
}