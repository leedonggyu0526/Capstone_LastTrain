using System.Collections.Generic;
using UnityEngine;

public class TrainEventCSVReader
{
    // 딕셔너리 구조체로 이벤트 저장
    // key : eventID, value : trainEvent
    public Dictionary<string, TrainEvent> trainEventDic = new Dictionary<string, TrainEvent>();
    
    public Dictionary<string, TrainEvent> ReadTrainEventCSV(TextAsset trainEventCSV)
    {
        Debug.Log("CSV 파일 읽기 시작");
        
        // 입력 검증
        if (trainEventCSV == null)
        {
            Debug.LogError("할당된 파일이 없습니다.");
            return null;
        }

        if (string.IsNullOrEmpty(trainEventCSV.text))
        {
            Debug.LogError("CSV 파일이 비어있습니다.");
            return null;
        }

        try
        {
            string[] lines = trainEventCSV.text.Split('\n');
            
            if (lines.Length <= 1)
            {
                Debug.LogError("CSV 파일에 데이터가 없습니다 (헤더만 존재).");
                return null;
            }
            
            // 첫 번째 줄은 헤더이므로 건너뜀
            for (int i = 1; i < lines.Length; i++)
            {   
                // 줄 단위로 읽어옴
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                
                // 줄을 콤마(,)로 나눔
                string[] values = line.Split(',');
                
                // 최소 필드 개수 확인
                if (values.Length < 9)
                {
                    Debug.LogWarning($"라인 {i + 1}: 필드 개수가 부족합니다. 필요: 9개, 현재: {values.Length}개");
                    continue;
                }

                // 이벤트 ID 검증
                string eventID = values[0]?.Trim();
                if (string.IsNullOrEmpty(eventID))
                {
                    Debug.LogWarning($"라인 {i + 1}: 이벤트 ID가 비어있습니다.");
                    continue;
                }
                
                // 중복 ID 검사
                if (trainEventDic.ContainsKey(eventID))
                {
                    Debug.LogWarning($"라인 {i + 1}: 중복된 이벤트 ID입니다: {eventID}");
                    continue;
                }
                
                // 이미지 로딩 시도
                Sprite eventImage = LoadEventImage(eventID);

                // 이벤트 데이터 생성
                TrainEvent eventData = new TrainEvent(
                    eventImage, 
                    values[1]?.Trim() ?? "", 
                    values[2]?.Trim() ?? "", 
                    values[3]?.Trim() ?? "", 
                    values[4]?.Trim() ?? "", 
                    values[5]?.Trim() ?? "", 
                    values[6]?.Trim() ?? "", 
                    values[7]?.Trim() ?? "", 
                    values[8]?.Trim() ?? ""
                );

                // 이벤트 데이터 추가
                trainEventDic.Add(eventID, eventData);
                Debug.Log($"이벤트 로드됨: {eventID} - {eventData.GetEventName()}");
            }
            
            Debug.Log($"총 {trainEventDic.Count}개의 이벤트를 로드했습니다.");
            return trainEventDic;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CSV 파일 읽기 중 오류 발생: {e.Message}");
            return null;
        }
    }
    
    // 이미지 캐시 딕셔너리 추가
    private static Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();
    
    private Sprite LoadEventImage(string eventID)
    {
        if (string.IsNullOrEmpty(eventID))
        {
            Debug.LogWarning("이벤트 ID가 비어있어서 이미지를 로드할 수 없습니다.");
            return null;
        }
        
        // 에디터가 아닌 경우에만 캐시 확인 (assertion 에러 방지)
        if (Application.isPlaying && imageCache.ContainsKey(eventID))
        {
            Debug.Log($"캐시에서 이미지 로드: {eventID}");
            return imageCache[eventID];
        }
        
        // 여러 경로에서 이미지 찾기 시도
        string[] possiblePaths = {
            $"Sprites/EventImages/{eventID}",
            $"Sprites/EventImages/{eventID.ToLower()}",
            $"Sprites/EventImages/{eventID.ToUpper()}",
        };
        
        Sprite loadedSprite = null;
        string successPath = "";
        
        try
        {
            foreach (string path in possiblePaths)
            {
                // 안전한 방식으로 리소스 로드
                loadedSprite = Resources.Load<Sprite>(path);
                if (loadedSprite != null)
                {
                    successPath = path;
                    break;
                }
            }
            
            if (loadedSprite != null)
            {
                // 플레이 모드에서만 캐시에 저장 (assertion 에러 방지)
                if (Application.isPlaying)
                {
                    imageCache[eventID] = loadedSprite;
                }
                Debug.Log($"✅ 이미지 로드 성공: {successPath}");
                return loadedSprite;
            }
            else
            {
                Debug.LogWarning($"⚠️ 이미지를 찾을 수 없습니다: {eventID}");
                Debug.LogWarning($"시도한 경로들:");
                foreach (string path in possiblePaths)
                {
                    Debug.LogWarning($"  - Resources/{path}");
                }
                Debug.LogWarning($"💡 해결 방법:");
                Debug.LogWarning($"  1. 이미지 파일이 'Assets/Resources/Sprites/EventImages/{eventID}.png' 경로에 있는지 확인");
                Debug.LogWarning($"  2. 이미지 파일명이 '{eventID}'와 정확히 일치하는지 확인 (대소문자 구분)");
                Debug.LogWarning($"  3. Unity에서 이미지의 Texture Type이 'Sprite (2D and UI)'로 설정되어 있는지 확인");
                
                // 플레이 모드에서만 캐시에 null 저장하여 중복 로딩 방지
                if (Application.isPlaying)
                {
                    imageCache[eventID] = null;
                }
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 이미지 로드 중 오류 발생: {e.Message}");
            Debug.LogError($"이벤트 ID: {eventID}");
            
            // 플레이 모드에서만 캐시에 null 저장
            if (Application.isPlaying)
            {
                imageCache[eventID] = null;
            }
            return null;
        }
    }
    
    // 이미지 캐시 관리 메서드들
    public static void ClearImageCache()
    {
        imageCache.Clear();
        Debug.Log("이미지 캐시가 초기화되었습니다.");
    }
    
    public static bool IsImageCached(string eventID)
    {
        return imageCache.ContainsKey(eventID);
    }
    
    public static int GetCacheSize()
    {
        return imageCache.Count;
    }
    
    public static void PreloadAllImages(Dictionary<string, TrainEvent> events)
    {
        Debug.Log("=== 모든 이벤트 이미지 사전 로딩 시작 ===");
        int loadedCount = 0;
        int failedCount = 0;
        
        foreach (var eventEntry in events)
        {
            string eventID = eventEntry.Key;
            if (!imageCache.ContainsKey(eventID))
            {
                var reader = new TrainEventCSVReader();
                Sprite sprite = reader.LoadEventImage(eventID);
                if (sprite != null)
                {
                    loadedCount++;
                }
                else
                {
                    failedCount++;
                }
            }
        }
        
        Debug.Log($"=== 이미지 사전 로딩 완료 ===");
        Debug.Log($"성공: {loadedCount}개, 실패: {failedCount}개, 캐시 크기: {imageCache.Count}개");
    }
} 