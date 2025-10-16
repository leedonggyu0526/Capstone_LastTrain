using UnityEngine;
using TMPro;

public class DistanceManager : MonoBehaviour
{
    [Header("Refs")]
    public CrossroadUI crossroadUI;
    public GearSpeedController gearSpeedController;
    public PausePlayController pausePlayController;
    public TMP_Text progressUI;

    [Header("Distance")]
    [SerializeField] private float totalDistance = 0f;

    [SerializeField] private bool showProgressLog = true;   // 인스펙터에서 켜기/끄기
    [SerializeField] private float progressLogInterval = 1f;
    private float progressLogTimer = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        totalDistance += gearSpeedController.currentSpeed * Time.deltaTime;
        progressUI.text = $"{totalDistance:F1} / {crossroadUI.CurrentTarget:F1} {totalDistance / crossroadUI.CurrentTarget * 100:F1}%";

        if (showProgressLog)
        {
            progressLogTimer += Time.deltaTime;
            if (progressLogTimer >= progressLogInterval)
            {
                string targetTxt = (crossroadUI != null && crossroadUI.IsInitialized)
                    ? crossroadUI.CurrentTarget.ToString("F1")
                    : "초기화 전";

                Debug.Log($"[진행도] 현재 {totalDistance:F1} / 목표 {targetTxt} (속도 {gearSpeedController.currentSpeed:F2}, 기어 {gearSpeedController.currentGear + 1})");

                // crossroadUI 참조/초기화 상태 확인
                if (crossroadUI == null) Debug.LogError("[진행도] crossroadUI 참조 NULL!");
                else if (!crossroadUI.IsInitialized) Debug.LogWarning("[진행도] crossroadUI 초기화 전!");

                progressLogTimer = 0f;
            }
        }

        // ★ 거리 전달 (여기서 실제 트리거가 일어남)
        if (crossroadUI != null){
            if(totalDistance >= crossroadUI.CurrentTarget)
            {
                Debug.Log("[DistanceManager] 목표 거리 도달");
                pausePlayController.TogglePause();
            }
            crossroadUI.CheckDistance(totalDistance);
        }else
            Debug.LogError("[Gear] crossroadUI가 연결되지 않아 CheckDistance 호출 불가");
    }

    public void ResetDistance()
    {
        totalDistance = 0f;
        Debug.Log("[이동 거리] 0으로 초기화");
    }
}
