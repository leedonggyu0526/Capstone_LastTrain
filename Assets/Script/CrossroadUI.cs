using UnityEngine;

public class CrossroadUI : MonoBehaviour
{
    public GameObject uiPopup;
    public float[] possibleDistances = { 200f, 300f, 400f, 500f };

    private float nextTriggerDistance;
    private bool hasInitialized = false;

    private void Start()
    {
        SetNextTriggerDistance(0f);
    }

    public void CheckDistance(float distance)
    {
        if (!hasInitialized) return;

        if (distance >= nextTriggerDistance)
        {
            ShowPopup();
            SetNextTriggerDistance(distance);
        }
    }

    void SetNextTriggerDistance(float currentDistance)
    {
        float offset = possibleDistances[Random.Range(0, possibleDistances.Length)];
        nextTriggerDistance = currentDistance + offset;
        hasInitialized = true;
        Debug.Log($"다음 이벤트 거리: {nextTriggerDistance}");
    }

    void ShowPopup()
    {
        uiPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        uiPopup.SetActive(false);
    }
}
