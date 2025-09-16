using UnityEngine;

public class GearSpeedController : MonoBehaviour
{
    public BackgroundScrolling backgroundScrolling;

    /// <summary>
    /// 기어 단계 (0 ~ 5)을 받아서 속도 조절
    /// </summary>
    public void SetGearSpeed(int gearLevel)
    {
        float[] speeds = { 1f, 2f, 3f, 4f, 5f, 6f }; // 원하는 속도
        if (gearLevel >= 0 && gearLevel < speeds.Length)
        {
            backgroundScrolling.speed = speeds[gearLevel];
            Debug.Log($"기어 {gearLevel + 1}단 → 속도 {speeds[gearLevel]}");
        }
    }
}
