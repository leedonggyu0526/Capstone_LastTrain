using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public Button continueButton;
    public Button mainMenuButton;
    public Button optionsButton;
    public Button exitButton; // ✅ 새로 추가된 부분

    void Start()
    {
        continueButton.onClick.AddListener(OnClickContinue);
        mainMenuButton.onClick.AddListener(OnClickMainMenu);
        optionsButton.onClick.AddListener(OnClickOptions);
        exitButton.onClick.AddListener(OnClickExit); // ✅ 새로 추가된 부분
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
        Time.timeScale = isActive ? 1f : 0f;
    }

    public void OnClickContinue()
    {
        ToggleSettings();
    }

    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    public void OnClickOptions()
    {
        Debug.Log("설정 버튼 클릭됨");
    }

    public void OnClickExit()
    {
        Debug.Log("게임 종료 시도 중...");
        Application.Quit();
    }
}
