using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가
using TMPro; // UI 텍스트 사용을 위해 추가

public class EventObject : MonoBehaviour
{
    // === 인스펙터 참조 ===
    [Header("이벤트 설정")]
    public int eventID; // 이벤트 ID (CardID와 매칭됨)
    public TextMeshProUGUI titleText; // 이벤트 제목을 표시할 UI 텍스트

    // === 내부 필드 ===
    // 이벤트 데이터 저장을 위한 필드
    private EventData currentData;
    // EventSpawner와의 통신을 위한 참조
    private EventSpawner eventSpawner;


    void Start()
    {
        gameObject.SetActive(false);
        // 씬에서 EventSpawner 인스턴스를 찾아 참조
        eventSpawner = FindObjectOfType<EventSpawner>();

        if (eventSpawner == null)
        {
            Debug.LogError("[EventObject] 씬에서 EventSpawner를 찾을 수 없습니다! 이벤트 종료 로직에 문제가 발생합니다.");
        }
    }

    /// <summary>
    /// EventSpawner에 의해 이벤트가 시작될 때 호출됩니다.
    /// </summary>
    public void ActivateEvent(EventData data)
    {
        currentData = data;
        gameObject.SetActive(true);

        // 텍스트 UI 업데이트 (EventData에 eventName이 있다고 가정)
        if (titleText != null)
        {
            titleText.text = currentData.eventName;
        }

        // 🚨 새로 추가된 로직: 지속 시간 확인 및 타이머 시작
        float duration = currentData.DurationValue;

        if (duration > 0f)
        {
            // 양수 duration: 시간 제한이 있으므로 타이머 코루틴 시작
            StartCoroutine(EventDurationTimer(duration));
        }
        else if (duration == 0f)
        {
            // DurationValue가 0이면 즉시 이벤트 종료 (예: 즉발성 장애물)
            // **TODO: 여기에서 즉시 효과 적용 로직을 호출해야 함**

            // EventSpawner에게 이벤트 종료 요청
            if (eventSpawner != null)
            {
                eventSpawner.DestroyCurrentEvent();
            }
        }
        // else (duration == -1f): 무한 지속 (INF)이므로 타이머를 시작하지 않음

        // **TODO: 여기에서 이벤트 효과 적용 로직(예: EventEffectManager 호출)을 시작해야 합니다.**
        // 예: EventEffectManager.Instance.ApplyEffect(currentData);
    }

    /// <summary>
    /// 플레이어가 카드를 사용하여 이벤트를 해결하거나, 타이머가 만료될 때 호출됩니다.
    /// </summary>
    public void DeactivateEvent()
    {
        // 🚨 중요: 카드로 해결될 경우, 실행 중인 자동 종료 타이머를 멈춥니다.
        StopAllCoroutines();

        gameObject.SetActive(false);

        // **TODO: 여기에서 이벤트 효과 해제 로직을 호출해야 합니다.**
        // 예: EventEffectManager.Instance.ClearEffect(currentData);

        // UI 텍스트 초기화 (옵션)
        if (titleText != null)
        {
            titleText.text = "";
        }
    }

    /// <summary>
    /// 이벤트 지속 시간을 카운트하고 시간이 다 되면 이벤트 종료를 요청합니다.
    /// </summary>
    private IEnumerator EventDurationTimer(float duration)
    {
        Debug.Log($"[EventObject] 이벤트 타이머 시작: {currentData.eventName} ({duration}초)");
        yield return new WaitForSeconds(duration);

        // 시간이 다 되면 EventSpawner에게 이벤트 종료를 요청
        if (eventSpawner != null)
        {
            eventSpawner.DestroyCurrentEvent();
            Debug.Log($"[EventObject] 시간 만료로 이벤트 종료: {currentData.eventName}");
        }
    }

    /// <summary>
    /// 현재 이벤트 데이터를 반환합니다.
    /// </summary>
    public EventData GetCurrentData()
    {
        return currentData;
    }
}