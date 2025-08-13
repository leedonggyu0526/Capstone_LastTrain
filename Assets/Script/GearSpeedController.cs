using UnityEngine;

public class GearSpeedController : MonoBehaviour
{
    public BackgroundScrolling backgroundScrolling;

    /// <summary>
    /// 기어 속도 (0 ~ 5) 설정
    /// </summary>
    public void SetGearSpeed(int gearLevel)
    {
        float[] speeds = { 1f, 2f, 3f, 4f, 5f, 6f }; // 기어 속도
        if (gearLevel >= 0 && gearLevel < speeds.Length)
        {
            backgroundScrolling.speed = speeds[gearLevel];
            Debug.Log($"기어 {gearLevel + 1}번 속도 {speeds[gearLevel]}");
        }
    }
}
