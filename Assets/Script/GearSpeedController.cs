using UnityEngine;
using System.Collections;

public class GearSpeedController : MonoBehaviour
{
    [Header("Refs")]
    public BackgroundScrolling backgroundScrolling;

    [Header("Gears (0 = 기본속도 1f)")]
    [SerializeField] private float[] speeds = { 1f, 2f, 3f, 4f, 5f, 6f };
    [SerializeField] private int defaultGearIndex = 0;

    public int currentGear = 0;
    public float currentSpeed = 0f;

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
        if (Time.timeScale == 0f) return;
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
}
