using UnityEngine;
using UnityEngine.UI; // Cần thiết cho các loại UI
using UnityEngine.SceneManagement; // RẤT QUAN TRỌNG cho SceneManager.LoadScene

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI; // Kéo PauseBackgroundPanel vào đây (hoặc PauseMenuPanel nếu bạn chỉ dùng 1 panel)

    private bool isPaused = false;

    // Hàm được gọi khi game bắt đầu
    void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Đảm bảo menu bị ẩn khi game bắt đầu
        }
        Time.timeScale = 1f; // Đảm bảo thời gian game chạy bình thường
    }

    // Hàm được gọi mỗi frame
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

    // Hàm này được Unity gọi khi ứng dụng được focus hoặc mất focus
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
            pauseMenuUI.SetActive(true); // Hiển thị menu tạm dừng
        }
        Time.timeScale = 0f; // Dừng thời gian trong game
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Ẩn menu tạm dừng
        }
        Time.timeScale = 1f; // Thời gian game chạy bình thường
        isPaused = false;
    }

    // --- HÀM MỚI (hoặc kiểm tra lại) CHO NÚT RESTART ---
    public void RestartGame()
    {
        // Điều này rất quan trọng: Đặt Time.timeScale về 1f trước khi tải lại scene,
        // nếu không, scene mới sẽ bị dừng ngay lập tức.
        Time.timeScale = 1f;

        // Tải lại scene hiện tại.
        // Để hàm này hoạt động, bạn cần đảm bảo scene của bạn đã được thêm vào Build Settings.
        // Vào File -> Build Settings... -> Kéo scene hiện tại của bạn vào danh sách "Scenes In Build".
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game Restarted!"); // Thông báo trong Console để kiểm tra
    }

    // --- HÀM MỚI (hoặc kiểm tra lại) CHO NÚT EXIT ---
    public void QuitGame()
    {
        Debug.Log("Quit Game!"); // Thông báo trong Console để kiểm tra
        Application.Quit(); // Hàm này chỉ hoạt động trong bản build game (exe, apk, v.v.),
                            // nó sẽ không làm gì khi bạn chạy trong Unity Editor.
                            // Trong Editor, bạn chỉ thấy thông báo "Quit Game!" trong Console.

        // Nếu bạn muốn dừng chơi trong Editor, bạn có thể dùng dòng dưới (chỉ để debug):
        // UnityEditor.EditorApplication.isPlaying = false;
        // Nhưng cần 'using UnityEditor;' và nó chỉ dành cho Editor, không dùng trong game thực tế.
    }
}