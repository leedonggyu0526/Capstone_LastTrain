using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 일시정지 메뉴 관리 : ESC 키로 토글, Resume, Main Menu, Exit 기능 포함
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // PauseMenuCanvas 연결
//    public GameObject backgroundDimmer; // Canvas의 윈도우 앞에 위치
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }


    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);

        UIDimmer.Instance.Hide(); // 범용 Dimmer를 숨깁니다.
        Time.timeScale = 1f;
        isPaused = false;
    }

    void PauseGame()
    {

        pauseMenuUI.SetActive(true);
        // 범용 Dimmer를 pauseMenuUI 뒤에 표시하도록 요청합니다.
        UIDimmer.Instance.Show(pauseMenuUI.transform);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene"); // 시작화면 씬 이름 정확히 써야 함
    }

    public void ExitGame()
    {
        Application.Quit(); // 빌드된 게임에서 작동
    }
}
