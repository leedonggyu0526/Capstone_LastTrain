using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlayController : MonoBehaviour
{
    [Header("UI Button 및 텍스트")]
    public Button toggleButton;      // Inspector에서 연결해야할 UI Button
    public TMP_Text buttonText;        // 버튼 안의 텍스트 Text (TextMeshPro 사용 시 TMP_Text)

    private bool isPaused = false;

    void Start()
    {
        // 버튼 클릭 시 TogglePause() 호출
        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePause);

        // 현재 상태에 따라 버튼 텍스트 업데이트
        UpdateButtonLabel();
    }

    void Update()
    {
        // 스페이스바 키를 누르면 일시 정지/재생 토글
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 일시 정지/재생 토글
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        // 시간 스케일 조정
        Time.timeScale = isPaused ? 0f : 1f;

        // 버튼 텍스트 업데이트
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
