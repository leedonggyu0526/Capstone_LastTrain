using UnityEngine;

// CSV 파일의 한 줄 데이터를 담는 클래스 (데이터 구조체)
[System.Serializable]
public class EventData
{
    // === CSV 컬럼과 1:1 매칭되는 필드 ===

    // 기본 정보
    public int eventID;                 // 이벤트의 고유 식별 번호 (Primary Key)
    public string eventName;            // 이벤트 이름 (UI 표시용)

    // 첫 번째 효과
    public string effectType_1;         // 효과 유형 (예: DAMAGE_DURATION)
    public float effectValue_1;         // 효과 수치 (예: 8.0, 0.25)
    public string effectTarget_1;       // 효과 대상 (예: TRAIN_HP, ENGINE)

    // 두 번째 효과 (복합 이벤트용)
    public string effectType_2;
    public float effectValue_2;
    public string effectTarget_2;

    // 지속 시간
    public string duration;             // 지속 시간 (숫자 문자열 또는 "INF")

    // === 편의를 위한 보조 속성 (Parsing 후 사용) ===

    // duration 문자열을 float으로 변환한 값 (INF는 -1 등으로 처리)
    public float DurationValue
    {
        get
        {
            if (duration.ToUpper() == EventEffectType.DURATION_INFINITE)
            {
                // 무한 지속을 나타내는 값
                return -1f;
            }
            if (float.TryParse(duration, out float result))
            {
                return result;
            }
            return 0f; // 파싱 실패 또는 즉시 발동
        }
    }

    public EventData() { }

    public override string ToString()
    {
        return $"[EventData ID: {eventID}, Name: {eventName}] " +
               $"Effect 1: {effectType_1} ({effectValue_1} on {effectTarget_1}), " +
               $"Duration: {duration}";
    }
}