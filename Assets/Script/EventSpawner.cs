using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    public EventObject[] eventObjects; // Inspector에 추가한 이벤트 오브젝트 배열
    public int currentEventID = -1;    // 현재 이벤트 ID (-1이면 이벤트 없음)

    private float timer = 0f;          // 타이머
    private float spawnInterval = 5f;  // 5초마다 이벤트 생성

    void Update()
    {
        // 타이머 증가
        timer += Time.deltaTime;

        // 5초마다 이벤트 생성
        if (timer >= spawnInterval)
        {
            TrySpawnEvent();
            timer = 0f; // 타이머 초기화
        }

        // P 키 입력 시 이벤트 생성
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnEvent();
        }
    }

    // 이벤트 생성 함수
    public void TrySpawnEvent()
    {
        // 이벤트가 이미 있으면 생성하지 않음
        if (currentEventID != -1)
        {
            Debug.Log("이벤트가 이미 있습니다.");
            return;
        }

        // 랜덤으로 이벤트 선택
        int randomIndex = Random.Range(0, eventObjects.Length);

        // 이벤트 활성화
        eventObjects[randomIndex].ActivateEvent();

        // 이벤트 ID 설정
        currentEventID = eventObjects[randomIndex].eventID;

        Debug.Log($"이벤트 생성: {currentEventID}");
    }

    // 이벤트 제거 함수
    public void DestroyCurrentEvent()
    {
        if (currentEventID == -1) return;

        // 이벤트 비활성화
        foreach (var evt in eventObjects)
        {
            if (evt.eventID == currentEventID)
            {
                evt.DeactivateEvent();
                break;
            }
        }

        currentEventID = -1;

        Debug.Log("이벤트가 제거되었습니다.");
    }
}
