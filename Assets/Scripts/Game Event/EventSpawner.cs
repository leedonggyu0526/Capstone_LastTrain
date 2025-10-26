using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    // EventObject[] 배열은 Inspector에 EventData의 ID와 일치하는 EventObject 프리팹을 등록해야 합니다.
    public EventObject[] eventObjects;
    public int currentEventID = -1;    // 현재 이벤트 ID (-1이면 이벤트 없음)

    private float timer = 0f;          // 타이머 변수
    private float spawnInterval = 5f;  // 5초 주기

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnEvent();
            timer = 0f; // 타이머 초기화
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TrySpawnEvent();
        }
    }

    // 이벤트 발생 함수 (데이터 전달 로직 추가됨)
    public void TrySpawnEvent()
    {
        if (currentEventID != -1)
        {
            Debug.Log("이미 이벤트가 발생 중입니다.");
            return;
        }

        if (EventDatabase.Instance == null)
        {
            Debug.LogError("EventDatabase가 씬에 없습니다. 이벤트를 발생시킬 수 없습니다.");
            return;
        }

        // 1. 랜덤 EventObject 선택
        int randomIndex = Random.Range(0, eventObjects.Length);
        int eventIDToSpawn = eventObjects[randomIndex].eventID;

        // 2. EventDatabase에서 상세 데이터 가져오기
        EventData eventData = EventDatabase.Instance.GetEventData(eventIDToSpawn);

        if (eventData == null)
        {
            Debug.LogError($"ID {eventIDToSpawn} 에 해당하는 EventData를 찾을 수 없습니다. CSV 확인 필요.");
            return;
        }

        // 3. 이벤트 활성화 및 데이터 전달 (EventObject.cs의 함수 시그니처 변경 반영)
        eventObjects[randomIndex].ActivateEvent(eventData);

        // 4. 현재 이벤트 ID 저장
        currentEventID = eventIDToSpawn;

        Debug.Log($"이벤트 발생: {eventData.eventName} (ID: {currentEventID})");
    }

    // 이벤트 삭제 함수
    public void DestroyCurrentEvent()
    {
        if (currentEventID == -1) return;

        // 현재 이벤트 비활성화
        foreach (var evt in eventObjects)
        {
            if (evt.eventID == currentEventID)
            {
                evt.DeactivateEvent();
                break;
            }
        }

        // 이벤트 ID 초기화
        currentEventID = -1;

        Debug.Log("이벤트가 종료되었습니다.");
    }
}