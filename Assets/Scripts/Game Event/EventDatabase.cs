using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 이벤트 데이터를 로드하고 저장하는 중앙 관리자
public class EventDatabase : MonoBehaviour
{
    public TextAsset eventCsvFile;

    private Dictionary<int, EventData> eventDataDictionary = new Dictionary<int, EventData>();

    public static EventDatabase Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadEventData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadEventData()
    {
        if (eventCsvFile == null)
        {
            Debug.LogError("EventDatabase: eventCsvFile이 Inspector에 할당되지 않았습니다. CSV 파일을 할당하세요.");
            return;
        }

        string csvText = eventCsvFile.text;
        // 윈도우/유닉스 개행 문자 모두 처리 (CRLF, LF)
        string[] lines = csvText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length > 1)
        {
            // 헤더(첫 줄) 제외하고 데이터 파싱
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // 콤마(,)로 필드를 분할합니다.
                string[] fields = line.Split(',');

                // CSV 구조: eventID, eventName, effectType_1, effectValue_1, ...
                if (fields.Length >= 9)
                {
                    EventData data = new EventData();

                    // ==========================================================
                    // 🚨 ID 파싱 및 오류 진단 강화 부분 (핵심 수정)
                    // ==========================================================
                    string idString = fields[0].Trim();

                    if (int.TryParse(idString, out data.eventID))
                    {
                        // 파싱 성공
                        data.eventName = fields[1].Trim();

                        // 효과 1
                        data.effectType_1 = fields[2].Trim();
                        // float.TryParse는 실패해도 0을 반환하므로 try/catch 생략
                        float.TryParse(fields[3].Trim(), out data.effectValue_1);
                        data.effectTarget_1 = fields[4].Trim();

                        // 효과 2
                        data.effectType_2 = fields[5].Trim();
                        float.TryParse(fields[6].Trim(), out data.effectValue_2);
                        data.effectTarget_2 = fields[7].Trim();

                        // 지속 시간
                        data.duration = fields[8].Trim();

                        if (!eventDataDictionary.ContainsKey(data.eventID))
                        {
                            eventDataDictionary.Add(data.eventID, data);
                        }
                        else
                        {
                            Debug.LogWarning($"EventDatabase: 중복된 Event ID가 발견되었습니다: {data.eventID}");
                        }
                    }
                    else
                    {
                        // 🚨 ID 파싱 실패 시 로그 출력
                        // 이 로그가 발생하면 CSV 파일의 해당 줄에 공백이나 숨겨진 문자가 있는 것입니다.
                        Debug.LogError($"CSV ID 파싱 실패! 줄 번호: {i + 1}, 시도한 문자열: '{idString}'");
                        Debug.LogError($"해당 줄 전체 데이터 (추가 진단용): {line}");
                    }
                }
                else
                {
                    Debug.LogWarning($"CSV 줄 {i + 1}의 필드 개수 부족 ({fields.Length}/9), 해당 줄: {line}");
                }
            }
        }

        Debug.Log($"EventDatabase: 총 {eventDataDictionary.Count}개의 이벤트 데이터 로드 완료. (기대하는 ID가 포함되었는지 확인하세요)");

        // 🚨 디버깅용: ID 2가 로드되었는지 최종 확인
        if (eventDataDictionary.ContainsKey(2))
        {
            Debug.Log($"EventDatabase: ID 2 이벤트 데이터 로드 확인 완료.");
        }
        else
        {
            // ID 2가 없다는 로그는 CSV에 ID 2가 없거나 파싱 실패로 등록되지 않았음을 의미합니다.
            Debug.LogWarning($"EventDatabase: ID 2는 최종 데이터베이스에 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// Event ID를 사용하여 특정 EventData 객체를 가져옵니다.
    /// </summary>
    public EventData GetEventData(int id)
    {
        if (eventDataDictionary.TryGetValue(id, out EventData data))
        {
            return data;
        }
        // 이 메시지가 뜨면 CSV 로드 후에도 해당 ID가 없었다는 뜻입니다.
        Debug.LogWarning($"EventDatabase: ID {id}에 해당하는 EventData를 찾을 수 없습니다.");
        return null;
    }
}