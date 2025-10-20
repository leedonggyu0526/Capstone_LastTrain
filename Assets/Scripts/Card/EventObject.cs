using UnityEngine;

public class EventObject : MonoBehaviour
{
    public int eventID; // 이벤트 ID

    // 처음엔 비활성화   
    void Start()
    {
        gameObject.SetActive(false);
    }

    // 이벤트 시작 시 호출 메서드
    public void ActivateEvent()
    {
        gameObject.SetActive(true);
    }

    // 이벤트 종료 시 호출
    public void DeactivateEvent()
    {
        gameObject.SetActive(false);
    }
}
