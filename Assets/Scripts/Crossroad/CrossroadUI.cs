// CrossroadUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossroadUI : MonoBehaviour
{
    [Header("UI")]
    public CrossroadUIController uiController;

    [Header("Offsets")]
    public float[] possibleDistances = { 200f, 300f, 400f, 600f };

    [Header("State")]
    [SerializeField] private float nextTriggerDistance = 0f;
    [SerializeField] private bool initialized = false;

    // 읽기용
    public bool IsInitialized => initialized;
    public float CurrentTarget => nextTriggerDistance;

    void Start()
    {
        SetNextTriggerDistance(0f);
        Debug.Log($"[CrossroadUI] Start → 다음 목표: {nextTriggerDistance}");
        if (uiController == null) Debug.LogError("[CrossroadUI] uiController가 NULL 입니다. (CrossroadUIController 연결 필수)");
    }

    public void CheckDistance(float totalDistance)
    {
        if (!initialized)
        {
            Debug.LogWarning("[CrossroadUI] 초기화 전(CheckDistance 호출됨).");
            return;
        }

        // 디버그: 매 1초에 한 번쯤만 찍고 싶으면 타이머로 감싸도 됨
        // Debug.Log($"[CrossroadUI] CheckDistance 호출: 현재={totalDistance:F1}, 목표={nextTriggerDistance:F1}, 패널활성={(uiController?.panel != null && uiController.panel.activeSelf)}");

        // (원하면) 패널 열려있을 때 중복 방지
        if (uiController != null && uiController.panel != null && uiController.panel.activeSelf)
            return;

        if (totalDistance >= nextTriggerDistance)
        {
            // Debug.Log($"[CrossroadUI] 목표 {nextTriggerDistance:F1} 도달 → UI 오픈 시도");
            // if (uiController != null)
            // {
            //     uiController.Show();
            // }
            // else
            // {
            //     Debug.LogError("[CrossroadUI] uiController가 NULL이라 Show() 호출 불가");
            // }
            SetNextTriggerDistance(totalDistance);
            SceneManager.LoadScene("Settlement");
        }
    }

    private void SetNextTriggerDistance(float currentDistance)
    {
        if (possibleDistances == null || possibleDistances.Length == 0)
            possibleDistances = new float[] { 200f, 300f, 400f, 600f };

        float offset = possibleDistances[Random.Range(0, possibleDistances.Length)];
        nextTriggerDistance = currentDistance + offset;
        initialized = true;

        Debug.Log($"[CrossroadUI] 다음 목표 설정: {nextTriggerDistance:F1} (오프셋 {offset})");
    }

    public void ResetFrom(float fromDistance = 0f) => SetNextTriggerDistance(fromDistance);
}
