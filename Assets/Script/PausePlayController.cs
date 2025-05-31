using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlayController : MonoBehaviour
{
    [Header("UI Button 및 라벨")]
    public Button toggleButton;      // Inspector에 드래그할 UI Button
    public TMP_Text buttonText;        // 버튼 위에 올릴 Text (TextMeshPro 사용 시 TMP_Text)

    private bool isPaused = false;

    void Start()
    {
        // 버튼 클릭 시 TogglePause() 호출
        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePause);

        // 초기 버튼 라벨 설정
        UpdateButtonLabel();
    }

    void Update()
    {
        // 스페이스바 토글
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 게임 일시정지/재생 토글
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        // 시간 흐름 제어
        Time.timeScale = isPaused ? 0f : 1f;

        // 버튼 라벨 갱신
        UpdateButtonLabel();
    }

    private void UpdateButtonLabel()
    {
        if (buttonText != null)
        {
            buttonText.text = isPaused ? "Play" : "Pause";
        }
    }
}
