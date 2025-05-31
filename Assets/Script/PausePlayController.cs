using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlayController : MonoBehaviour
{
    [Header("UI Button �� ��")]
    public Button toggleButton;      // Inspector�� �巡���� UI Button
    public TMP_Text buttonText;        // ��ư ���� �ø� Text (TextMeshPro ��� �� TMP_Text)

    private bool isPaused = false;

    void Start()
    {
        // ��ư Ŭ�� �� TogglePause() ȣ��
        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePause);

        // �ʱ� ��ư �� ����
        UpdateButtonLabel();
    }

    void Update()
    {
        // �����̽��� ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// ���� �Ͻ�����/��� ���
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        // �ð� �帧 ����
        Time.timeScale = isPaused ? 0f : 1f;

        // ��ư �� ����
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
