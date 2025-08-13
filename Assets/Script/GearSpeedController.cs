using UnityEngine;
using System.Collections;

public class GearSpeedController : MonoBehaviour
{
    [Header("Refs")]
    public BackgroundScrolling backgroundScrolling;
    public CrossroadUI crossroadUI;   // ★ 예전 트리거를 여기 연결

    [Header("Gears (0 = 기본속도 1f)")]
    [SerializeField] private float[] speeds = { 1f, 2f, 3f, 4f, 5f, 6f };
    [SerializeField] private int defaultGearIndex = 0;

    private int currentGear = 0;
    private float currentSpeed = 0f;

    [Header("Distance")]
    [SerializeField] private float totalDistance = 0f;
    [SerializeField] private bool logEverySecond = true;
    private float logTimer = 0f;

    [SerializeField] private bool showProgressLog = true;   // 인스펙터에서 켜기/끄기
    [SerializeField] private float progressLogInterval = 1f;
    private float progressLogTimer = 0f;

    void Start()
    {
        SetGearSpeed(defaultGearIndex);
        StartCoroutine(ApplySpeedNextFrame());
        Debug.Log($"[Gear] 시작: speeds[0]={speeds[0]}");
    }

    IEnumerator ApplySpeedNextFrame()
    {
        yield return null;
        ApplySpeed();
    }

    void Update()
    {
        // GearSpeedController.cs - Update() 안
        if (Time.timeScale == 0f) return;

        totalDistance += currentSpeed * Time.deltaTime;

        // ✅ 1초 주기로 "현재/목표" 찍기
        if (showProgressLog)
        {
            progressLogTimer += Time.deltaTime;
            if (progressLogTimer >= progressLogInterval)
            {
                string targetTxt = (crossroadUI != null && crossroadUI.IsInitialized)
                    ? crossroadUI.CurrentTarget.ToString("F1")
                    : "초기화 전";

                Debug.Log($"[진행도] 현재 {totalDistance:F1} / 목표 {targetTxt} (속도 {currentSpeed:F2}, 기어 {currentGear + 1})");

                // crossroadUI 참조/초기화 상태 확인
                if (crossroadUI == null) Debug.LogError("[진행도] crossroadUI 참조 NULL!");
                else if (!crossroadUI.IsInitialized) Debug.LogWarning("[진행도] crossroadUI 초기화 전!");

                progressLogTimer = 0f;
            }
        }

        // ★ 거리 전달 (여기서 실제 트리거가 일어남)
        if (crossroadUI != null)
            crossroadUI.CheckDistance(totalDistance);
        else
            Debug.LogError("[Gear] crossroadUI가 연결되지 않아 CheckDistance 호출 불가");
    }



    public void SetGearSpeed(int gearIndex)
    {
        if (gearIndex < 0 || gearIndex >= speeds.Length)
        {
            Debug.LogWarning($"[기어 변경] 잘못된 인덱스: {gearIndex}");
            return;
        }
        currentGear = gearIndex;
        ApplySpeed();
    }

    private void ApplySpeed()
    {
        if (speeds == null || speeds.Length == 0) speeds = new float[] { 1f, 2f, 3f, 4f };
        if (speeds[0] <= 0f) { Debug.LogWarning("[Diag] speeds[0]를 1f로 보정"); speeds[0] = 1f; }

        currentSpeed = speeds[currentGear];

        if (backgroundScrolling != null)
        {
            backgroundScrolling.speed = currentSpeed;
            Debug.Log($"[기어 변경] {currentGear + 1}단 → 속도 {currentSpeed}");
        }
        else
        {
            Debug.LogError("[Gear] BackgroundScrolling 미연결");
        }
    }

    public void ResetDistance()
    {
        totalDistance = 0f;
        Debug.Log("[이동 거리] 0으로 초기화");
    }
}
