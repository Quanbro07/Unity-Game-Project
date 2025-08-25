using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // RẤT QUAN TRỌNG: Import thư viện UI để sử dụng Image và Text
using UnityEngine.SceneManagement; // Import thư viện quản lý Scene (cho ExitGame)

public class PlayerController : MonoBehaviour
{
    // =========================================================
    // HỆ THỐNG MÁU
    // =========================================================
    [Header("Health System")]
    [SerializeField] private Health health;
    [SerializeField] private GameObject gameOverUI;
    public void PlayerDied()
    {
        // Hiển thị panel Game Over
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Tạm dừng game
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Tiếp tục game
        Time.timeScale = 1;
        // Tải lại màn chơi hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    [Header("UI Hearts")]
    [SerializeField] private Image[] heartImages; // Kéo 3 đối tượng trái tim vào đây
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [Header("Level Indicator")]
    [SerializeField] private TextMeshProUGUI levelTextIndicator;

    // =========================================================
    // HỆ THỐNG KINH NGHIỆM VÀ CẤP ĐỘ
    // =========================================================
    [Header("Experience System")]
    public int currentLevel = 1;
    public float currentExperience = 0f;
    public float experienceToNextLevel = 100f;

    // THAY ĐỔI: Sử dụng Image thay vì Slider cho thanh kinh nghiệm
    [SerializeField] private Image experienceBarFillImage; // Kéo ExpBarFill (Image) vào đây trong Inspector

    // Giữ hoặc xóa các biến Text này tùy thuộc vào việc bạn có đối tượng Text UI riêng biệt để hiển thị Level và EXP số hay không:
    [SerializeField] private Text levelText;     // Kéo đối tượng Text UI cho Level vào đây (Tùy chọn)
    [SerializeField] private Text expText;       // Kéo đối tượng Text UI cho EXP số vào đây (Tùy chọn)

    [SerializeField] private float experienceGainMultiplier = 1f;
    [SerializeField] private float expIncreasePerLevel = 1.2f;

    // =========================================================
    // HỆ THỐNG TẠM DỪNG GAME
    // =========================================================
    [Header("Pause System")]
    [SerializeField] private GameObject pauseMenuPanel; // Kéo GameObject PauseMenuPanel vào đây
    private bool isGamePaused = false; // Trạng thái game có đang tạm dừng hay không

    // =========================================================
    // HIỆU ỨNG LEVEL UP
    // =========================================================
    [Header("Level Up Effect")]
    [SerializeField] private GameObject levelUpEffectGameObject; // Kéo GameObject LevelUpEffect vào đây
    private Animator levelUpEffectAnimator; // Animator của hiệu ứng

    // =========================================================
    // CÁC BIẾN HIỆN CÓ CỦA PLAYER
    // =========================================================
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject weaponHolder;
    [SerializeField] private GameObject AttackArea;
    bool isFacingLeft = true; // Cảnh báo CS0414 sẽ vẫn xuất hiện nếu biến này không được sử dụng.

    #region Movement
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.zero;
    #endregion

    #region Dash
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashPower = 10f;
    [SerializeField] private float dashTime = 0.4f;
    [SerializeField] private float dashCoolDown = 1f;
    #endregion

    [SerializeField] private KnockBack knockBack; // Đảm bảo bạn đã có script KnockBack
    [SerializeField] private PlayerAttack PlayerAttack; // Đảm bảo bạn đã có script PlayerAttack

    #region Animation
    private int animaitonChoice = Animator.StringToHash("PlayerIdleRight");
    private int playerMoveRight = Animator.StringToHash("PlayerMoveRight");
    private int playerMoveUp = Animator.StringToHash("PlayerMoveUp");
    private int playerDash = Animator.StringToHash("PlayerDash");
    private int playerIdleRight = Animator.StringToHash("PlayerIdleRight");
    private int playerIdleUp = Animator.StringToHash("PlayerIdleUp");
    private int playerIdleDown = Animator.StringToHash("PlayerIdleDown");
    #endregion


    // =========================================================
    // HÀM LIFECYCLE UNITY
    // =========================================================
    void Awake()
    {
        // Đảm bảo các tham chiếu quan trọng không null
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (PlayerAttack == null) Debug.LogWarning("PlayerAttack script not assigned to PlayerController!");
        if (knockBack == null) Debug.LogWarning("KnockBack script not assigned to PlayerController!");

        // Lấy tham chiếu đến Animator của hiệu ứng Level Up
        if (levelUpEffectGameObject != null)
        {
            levelUpEffectAnimator = levelUpEffectGameObject.GetComponent<Animator>();
            if (levelUpEffectAnimator == null)
            {
                Debug.LogWarning("Level Up Effect GameObject does not have an Animator component!");
            }
        }
        else
        {
            Debug.LogWarning("Level Up Effect GameObject is not assigned in PlayerController!");
        }
    }

    void Start()
    {
        health.OnHealthChanged += UpdateHeartUI;
        UpdateHeartUI(healthCurrent: 5, maxHealth: 100);
        // GameManagerController.Instance.UpdatePlayerHPUI(currentHp, maxHp); // Nếu GameManager quản lý UI HP

        // Khởi tạo hệ thống kinh nghiệm
        InitializeExperienceSystem();

        // Đảm bảo Pause Menu ẩn khi game bắt đầu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause Menu Panel is not assigned to PlayerController! Please assign it in the Inspector.");
        }

        // Đảm bảo hiệu ứng level up ẩn khi bắt đầu game
        if (levelUpEffectGameObject != null)
        {
            levelUpEffectGameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Xử lý tạm dừng game đầu tiên
        HandlePauseInput();

        // Nếu game đang tạm dừng, không xử lý input di chuyển, dash, tấn công
        if (isGamePaused) return;

        // Các hàm xử lý input và logic game bình thường
        GatherInput();
        HandleDash();
        // Giả sử PlayerAttack.HandleAttackInput() cũng được gọi ở đây hoặc trong một hàm khác
        // PlayerAttack.HandleAttackInput(); // Nếu bạn có hàm riêng để gọi từ Update
    }

    private void FixedUpdate()
    {
        // Nếu game đang tạm dừng, không xử lý FixedUpdate
        if (isGamePaused) return;

        UpdateMovement();
        UpdateDirection();
    }

    // =========================================================
    // HỆ THỐNG TẠM DỪNG GAME
    // =========================================================
    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Nhấn phím Escape để tạm dừng/tiếp tục
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true); // Hiển thị Pause Menu
        }
        Time.timeScale = 0f; // Dừng thời gian trong game
        // Vô hiệu hóa input của player khi pause
        rb.linearVelocity = Vector2.zero; // Đảm bảo player dừng lại ngay lập tức
        animator.speed = 0f; // Dừng animation
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Ẩn Pause Menu
        }
        Time.timeScale = 1f; // Tiếp tục thời gian trong game
        // Kích hoạt lại animation
        animator.speed = 1f;
    }

    // =========================================================
    // HÀM QUẢN LÝ KINH NGHIỆM VÀ CẤP ĐỘ
    // =========================================================
    void InitializeExperienceSystem()
    {
        UpdateExperienceUI();
    }

    public void AddExperience(int amount)
    {
        float actualAmount = amount * experienceGainMultiplier;
        currentExperience += actualAmount;
        Debug.Log($"Player gained {actualAmount} experience! Current EXP: {currentExperience}/{experienceToNextLevel}");
        UpdateExperienceUI();
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            currentLevel++;
            Debug.Log($"Player Leveled Up! New Level: {currentLevel}");
            experienceToNextLevel = Mathf.Round(experienceToNextLevel * expIncreasePerLevel);
            UpdateExperienceUI();
            // GỌI HÀM KÍCH HOẠT HIỆU ỨNG KHI LÊN CẤP:
            ActivateLevelUpEffect();
            // TODO: Thêm các hiệu ứng khác khi lên cấp (ví dụ: hồi máu, tăng chỉ số, hiệu ứng âm thanh)
        }
    }

    void UpdateExperienceUI()
    {
        if (experienceBarFillImage != null)
        {
            experienceBarFillImage.fillAmount = currentExperience / experienceToNextLevel;
        }
        else
        {
            Debug.LogWarning("Experience Bar Fill Image (ExpBarFill) is not assigned to PlayerController!");
        }

        // Cập nhật Text cho level và exp hiện tại nếu bạn có các đối tượng Text UI riêng biệt
        // THAY ĐỔI: Sử dụng levelTextIndicator mới
        if (levelTextIndicator != null)
        {
            levelTextIndicator.text = currentLevel.ToString(); // Đặt giá trị Text bằng cấp độ hiện tại
        }

        if (expText != null)
        {
            expText.text = $"{Mathf.FloorToInt(currentExperience)} / {Mathf.FloorToInt(experienceToNextLevel)}";
        }
    }

    // =========================================================
    // HỆ THỐNG MÁU
    // =========================================================


    void UpdateHeartUI(int healthCurrent, int maxHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < healthCurrent)
                heartImages[i].sprite = fullHeartSprite;
            else
                heartImages[i].sprite = emptyHeartSprite;
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // TODO: Thêm logic khi player chết
        // Dừng game khi player chết
        Time.timeScale = 0f;
        // Tùy chọn: Vô hiệu hóa Player để không có tương tác nào nữa
        // gameObject.SetActive(false);
    }

    // =========================================================
    // HÀM DI CHUYỂN & DASH
    // =========================================================
    private void GatherInput()
    {
        // Chỉ nhận input nếu game KHÔNG tạm dừng
        if (isGamePaused || (knockBack != null && knockBack.IsKnocked)) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void HandleDash()
    {
        // Chỉ dash nếu game KHÔNG tạm dừng
        if (isGamePaused) return;

        if (Input.GetMouseButtonDown(1) && canDash && movement != Vector2.zero)
        {
            UpdateAnimation(playerDash);
            StartCoroutine(Dash());
        }
    }

    private void UpdateMovement()
    {
        // Chỉ di chuyển nếu game KHÔNG tạm dừng
        if (isGamePaused || isDashing || (knockBack != null && knockBack.IsKnocked) || (PlayerAttack != null && PlayerAttack.IsAttack)) return;

        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    private void UpdateDirection()
    {
        // Chỉ cập nhật hướng nếu game KHÔNG tạm dừng
        if (isGamePaused || isDashing || (knockBack != null && knockBack.IsKnocked) || (PlayerAttack != null && PlayerAttack.IsAttack))
        {
            return;
        }

        if (movement.x == 0 && movement.y == 0)
        {
            if (lastDirection != Vector2.zero && lastDirection == Vector2.up)
            {
                animaitonChoice = playerIdleUp;
            }
            else
            {
                animaitonChoice = playerIdleRight;
            }
        }
        else
        {
            if (movement.x < 0)
            {
                animaitonChoice = playerMoveRight;
                spriteRenderer.flipX = true;
                if (weaponHolder != null) weaponHolder.transform.localScale = new Vector3(-1, 1, 1);
                if (AttackArea != null) AttackArea.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (movement.x > 0)
            {
                spriteRenderer.flipX = false;
                animaitonChoice = playerMoveRight;
                if (weaponHolder != null) weaponHolder.transform.localScale = new Vector3(1, 1, 1);
                if (AttackArea != null) AttackArea.transform.localScale = new Vector3(1, 1, 1);
            }

            if (movement.y > 0)
            {
                animaitonChoice = playerMoveUp;
            }
            else if (movement.y < 0)
            {
                animaitonChoice = playerMoveRight;
            }
            lastDirection = movement;
        }
        UpdateAnimation(animaitonChoice);
    }

    private IEnumerator Dash()
    {
        // Chỉ dash nếu game KHÔNG tạm dừng
        if (isGamePaused) yield break;

        canDash = false;
        isDashing = true;

        rb.linearVelocity = movement.normalized * dashPower;
        yield return new WaitForSeconds(dashTime);

        isDashing = false;

        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    private void UpdateAnimation(int animation)
    {
        // Chỉ cập nhật animation nếu game KHÔNG tạm dừng
        if (isGamePaused || isDashing || (PlayerAttack != null && PlayerAttack.IsAttack) || (knockBack != null && knockBack.IsKnocked)) return;

        animator.CrossFade(animation, 0);
    }

    // =========================================================
    // HÀM KÍCH HOẠT HIỆU ỨNG LEVEL UP
    // =========================================================
    void ActivateLevelUpEffect()
    {
        if (levelUpEffectGameObject != null && levelUpEffectAnimator != null)
        {
            // ĐẶT VỊ TRÍ HIỆU ỨNG NGAY TẠI TÂM CỦA PLAYER
            // Đảm bảo pivot của các sprite hiệu ứng đã được đặt ở Center trong Sprite Editor.
            // Nếu Player có pivot ở chân, bạn có thể cần thêm một offset nhỏ theo trục Y.
            levelUpEffectGameObject.transform.position = transform.position; // Đặt vị trí hiệu ứng bằng với vị trí của Player

            levelUpEffectGameObject.SetActive(true); // Bật GameObject hiệu ứng
            // Chắc chắn rằng tên animation trong Play() khớp với tên clip animation bạn đã tạo (ví dụ: Player_LevelUp_Effect)
            levelUpEffectAnimator.Play("Player_LevelUp_Effect"); // Bắt đầu phát animation

            // Bắt đầu Coroutine để tắt hiệu ứng sau khi animation kết thúc
            // Sử dụng stateInfo.length để lấy thời gian thực của animation
            AnimatorStateInfo stateInfo = levelUpEffectAnimator.GetCurrentAnimatorStateInfo(0);
            StartCoroutine(DeactivateLevelUpEffectAfterDelay(stateInfo.length));
        }
    }

    IEnumerator DeactivateLevelUpEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Chờ cho đến khi animation kết thúc
        if (levelUpEffectGameObject != null)
        {
            levelUpEffectGameObject.SetActive(false); // Tắt GameObject hiệu ứng
        }
    }

    // =========================================================
    // HÀM THOÁT GAME
    // =========================================================
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        // Đảm bảo game không bị dừng khi thoát để tránh lỗi trong Editor
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Thoát khỏi Play Mode trong Editor
#else
        Application.Quit(); // Thoát ứng dụng khi build
#endif
    }
}