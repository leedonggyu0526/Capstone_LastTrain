using UnityEngine;

public class CrossroadEventTrigger : MonoBehaviour
{
    [Header("UI to open when reached")]
    public CrossroadUIController ui;         // 도달 시 열 UI 컨트롤러

    [Header("Next target offsets (pick one randomly)")]
    public float[] possibleOffsets = { 50f, 100f, 150f, 200f };

    [Header("State (read-only)")]
    [SerializeField] private float nextTargetDistance = 0f;
    [SerializeField] private bool initialized = false;

    private void Start()
    {
        SetNextTarget(0f);
    }

    /// <summary>외부(예: GearSpeedController)에서 현재 누적 거리를 계속 전달해줌</summary>
    public void CheckDistance(float totalDistance)
    {
        if (!initialized) return;

        // UI가 이미 열려있으면 중복 트리거 방지 (원하지 않으면 이 줄 삭제)
        if (ui != null && ui.gameObject.activeInHierarchy && ui.panel != null && ui.panel.activeSelf)
            return;

        if (totalDistance >= nextTargetDistance)
        {
            Debug.Log($"[CrossroadTrigger] 목표 {nextTargetDistance:F1} 도달 → UI 오픈");
            if (ui != null) ui.Show();
            SetNextTarget(totalDistance);
        }
    }

    /// <summary>다음 목표 거리를 현재 거리 + (200/300/400/600 중 랜덤)으로 설정</summary>
    private void SetNextTarget(float currentDistance)
    {
        if (possibleOffsets == null || possibleOffsets.Length == 0)
        {
            possibleOffsets = new float[] { 50f, 100f, 150f, 200f };
        }

        float offset = possibleOffsets[Random.Range(0, possibleOffsets.Length)];
        nextTargetDistance = currentDistance + offset;
        initialized = true;

        Debug.Log($"[CrossroadTrigger] 다음 목표 설정: {nextTargetDistance:F1} (오프셋 {offset})");
    }

    /// <summary>원하면 외부에서 다음 목표를 강제로 리셋</summary>
    public void ResetNextTarget(float fromDistance = 0f)
    {
        SetNextTarget(fromDistance);
    }
}
