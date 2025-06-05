using UnityEngine;

public class GearSpeedController : MonoBehaviour
{
    public BackgroundScrolling backgroundScrolling;

    /// <summary>
    /// ��� �ܰ� (0 ~ 5)�� �޾Ƽ� �ӵ� ����
    /// </summary>
    public void SetGearSpeed(int gearLevel)
    {
        float[] speeds = { 1f, 2f, 3f, 4f, 5f, 6f }; // ���ϴ� �ӵ�
        if (gearLevel >= 0 && gearLevel < speeds.Length)
        {
            backgroundScrolling.speed = speeds[gearLevel];
            Debug.Log($"��� {gearLevel + 1}�� �� �ӵ� {speeds[gearLevel]}");
        }
    }
}
