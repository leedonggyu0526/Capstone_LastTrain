using UnityEngine;
public class GearSpeedController : MonoBehaviour
{
    public BackgroundScrolling backgroundScrolling;
    public CrossroadUI crossroadUI;

    private float totalDistance = 0f;

    public void SetGearSpeed(int gearLevel)
    {
        float[] speeds = { 1f, 2f, 3f, 4f, 5f, 6f };
        if (gearLevel >= 0 && gearLevel < speeds.Length)
        {
            backgroundScrolling.speed = speeds[gearLevel];
        }
    }

    private void Update()
    {
        float distanceThisFrame = backgroundScrolling.speed * Time.deltaTime;
        totalDistance += distanceThisFrame;

        crossroadUI.CheckDistance(totalDistance);

        Debug.Log($"[이동 거리] {totalDistance:F2}m");
    }
}
