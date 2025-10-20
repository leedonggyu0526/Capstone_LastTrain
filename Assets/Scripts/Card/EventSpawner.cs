using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    public EventObject[] eventObjects; // Inspector에 등록할 이벤트 오브젝트 배열
    public int currentEventID = -1;    // 현재 이벤트 ID (-1이면 이벤트 없음)

    private float timer = 0f;          // 타이머 변수
    private float spawnInterval = 5f;  // 5초 주기

    void Update()
    {
        // 타이머 증가
        timer += Time.deltaTime;

        // 주기마다 이벤트 발생 시도
        if (timer >= spawnInterval)
        {
            TrySpawnEvent();
            timer = 0f; // 타이머 초기화
        }

        // P 키를 누르면 즉시 이벤트 발생 시도 (디버그/강제 이벤트용)
        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnEvent();
        }
    }

    // 이벤트 발생 함수
    public void TrySpawnEvent()
    {
        // 현재 이벤트가 존재하면 새로 발생하지 않음 (중복 방지)
        if (currentEventID != -1)
        {
            Debug.Log("이미 이벤트가 발생 중입니다.");
            return;
        }

        // 랜덤 이벤트 선택 (eventObjects 배열에서 하나 고름)
        int randomIndex = Random.Range(0, eventObjects.Length);

        // 이벤트 활성화 (EventObject 내부의 UI/애니메이션/텍스트 표시 등 실행)
        eventObjects[randomIndex].ActivateEvent();

        // 현재 이벤트 ID 저장 → DropZone과 매칭할 때 사용
        currentEventID = eventObjects[randomIndex].eventID;

        Debug.Log($"이벤트 발생: {currentEventID}");
    }

    // 이벤트 삭제 함수
    public void DestroyCurrentEvent()
    {
        if (currentEventID == -1) return; // 이벤트가 없으면 무시

        // 현재 이벤트 비활성화
        foreach (var evt in eventObjects)
        {
            if (evt.eventID == currentEventID)
            {
                evt.DeactivateEvent();   // EventObject 쪽에서 UI 제거/숨김
                break;
            }
        }

        // 이벤트 ID 초기화 → 다시 이벤트를 생성할 수 있게 됨
        currentEventID = -1;

        Debug.Log("이벤트가 종료되었습니다.");
    }
}
