using UnityEngine;

public class EventObject : MonoBehaviour
{
    public int eventID; // 이벤트 ID

    // 이벤트 활성화
    void Start()
    {
        gameObject.SetActive(false);
    }

    // 이벤트 활성화
    public void ActivateEvent()
    {
        gameObject.SetActive(true);
    }

    // 이벤트 비활성화
    public void DeactivateEvent()
    {
        gameObject.SetActive(false);
    }
}
