using UnityEngine;
using System.Collections.Generic;
using System;

// TrainEvent를 관리
// EventSystem 오브젝트에 추가해 사용
public class TrainEventManager : MonoBehaviour
{
    // 싱글톤 패턴 적용
    private static TrainEventManager trainEventManagerInstance;
    // TrainEventManager 프로퍼티
    public static TrainEventManager Instance
    {
        get
        {   
            if (trainEventManagerInstance == null)
            {
                // trainEventManagerInstance가 없으면 생성
                Debug.Log("TrainEventManager 인스턴스 생성");
                trainEventManagerInstance = FindFirstObjectByType<TrainEventManager>();
            }
            return trainEventManagerInstance;
        }
    }
    
    public TrainEventCSVReader trainEventCSVReader;
    // <이벤트 ID, 이벤트 객체> 형태의 딕셔너리
    private Dictionary<string, TrainEvent> trainEventDic;
    // trainEventCSV는 유니티 상에서 추가해 사용
    public TextAsset trainEventCSV;
    
    // 이벤트 체인 검증 결과 저장
    private Dictionary<string, bool> eventChainValidation;
    private List<string> unreachableEvents;
    private List<string> invalidReferences;
    private List<string> invalidRequirements;
    private List<string> invalidResults;
    
    void Awake()
    {
        // 싱글톤 인스턴스 관리
        if (trainEventManagerInstance == null)
        {
            trainEventManagerInstance = this;
            
            // 런타임에 생성된 GameObject에 HideFlags 설정
            if (gameObject.hideFlags == HideFlags.None)
            {
                gameObject.hideFlags = HideFlags.DontSave;
            }
            
            InitializeEventSystem();
        }
        else if (trainEventManagerInstance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void InitializeEventSystem()
    {
        Debug.Log("=== TrainEventManager 초기화 시작 ===");
        Debug.Log($"GameObject 이름: {gameObject.name}");
        
        // trainEventCSV가 할당되지 않았으면 오류 메시지 출력
        if (trainEventCSV == null)
        {
            Debug.LogError("trainEventCSV가 할당되지 않았습니다! Inspector에서 CSV 파일을 할당해주세요.");
            return;
        }
        else
        {
            // trainEventCSV가 할당되었으면 내용 확인   
            Debug.Log($"CSV 파일이 할당됨: {trainEventCSV.name}");
            Debug.Log($"CSV 파일 내용 길이: {trainEventCSV.text.Length}");
            Debug.Log($"CSV 파일 첫 100자: {trainEventCSV.text.Substring(0, Mathf.Min(100, trainEventCSV.text.Length))}");
        }
        
        // trainEventCSVReader가 할당되지 않았으면 생성
        if (trainEventCSVReader == null)
        {
            Debug.Log("TrainEventCSVReader 인스턴스 생성");
            trainEventCSVReader = new TrainEventCSVReader();
        }
        
        LoadCSVFile();
        Debug.Log("현재 로드된 이벤트 수: " + (trainEventDic?.Count ?? 0));
        
        if (trainEventDic != null && trainEventDic.Count > 0)
        {
            Debug.Log("로드된 이벤트 ID들:");
            foreach (var key in trainEventDic.Keys)
            {
                Debug.Log($"- {key}");
            }
            
            // 이벤트 체인 검증 실행
            ValidateEventChains();
            
            // CSV와 이미지 로딩이 완료된 후 DontDestroyOnLoad 적용
            if (Application.isPlaying)
            {
                try
                {
                    // 런타임에 생성된 GameObject에 적절한 HideFlags 설정
                    if (gameObject.hideFlags == HideFlags.None)
                    {
                        gameObject.hideFlags = HideFlags.DontSave;
                    }
                    DontDestroyOnLoad(gameObject);
                    Debug.Log("DontDestroyOnLoad 적용 완료");
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"DontDestroyOnLoad 적용 중 오류 발생: {ex.Message}");
                    Debug.Log("DontDestroyOnLoad 없이 계속 진행합니다.");
                }
            }
            
            // 모든 이벤트 이미지 사전 로딩을 코루틴으로 지연 실행
            StartCoroutine(DelayedImagePreload());
        }
        else
        {
            Debug.LogWarning("로드된 이벤트가 없습니다!");
            if (trainEventDic == null)
            {
                Debug.LogError("trainEventDic이 null입니다!");
            }
            else
            {
                Debug.LogError($"trainEventDic 카운트: {trainEventDic.Count}");
            }
        }
        Debug.Log("=== TrainEventManager 초기화 완료 ===");
    }

    void Start()
    {   
        Debug.Log("TrainEventManager Start - CSV 이미 로드됨");
        Debug.Log($"Start에서 확인한 이벤트 수: {GetEventCount()}");
    }
    
    public void LoadCSVFile()
    {
        Debug.Log("CSV 파일 로딩 시작");
        
        if (trainEventCSV == null)
        {
            Debug.LogError("CSV 파일이 null입니다!");
            return;
        }
        
        if (trainEventCSVReader == null)
        {
            Debug.LogError("trainEventCSVReader가 null입니다!");
            return;
        }
        
        trainEventDic = trainEventCSVReader.ReadTrainEventCSV(trainEventCSV);
        
        if (trainEventDic == null)
        {
            Debug.LogError("CSV 읽기 실패! trainEventDic이 null로 반환됨");
        }
        else
        {
            Debug.Log($"CSV 읽기 성공! {trainEventDic.Count}개 이벤트 로드됨");
        }
    }

    public TrainEvent GetTrainEvent(string eventID)
    {
        if (string.IsNullOrEmpty(eventID))
        {
            Debug.LogError("이벤트 ID가 비어있습니다.");
            return null;
        }
        
        if (trainEventDic != null && trainEventDic.ContainsKey(eventID))
        {
            var trainEvent = trainEventDic[eventID];
            
            // 이벤트 유효성 검사
            if (trainEvent != null && trainEvent.IsValidEvent())
            {
                return trainEvent;
            }
            else
            {
                Debug.LogWarning($"이벤트 ID '{eventID}'는 유효하지 않은 데이터를 포함하고 있습니다.");
                return trainEvent; // 유효하지 않더라도 반환 (사용자가 결정)
            }
        }
        else
        {
            Debug.LogWarning($"이벤트 ID '{eventID}'를 찾을 수 없습니다.");
            if (trainEventDic != null && trainEventDic.Count > 0)
            {
                Debug.LogWarning($"현재 사용 가능한 이벤트 ID: {string.Join(", ", trainEventDic.Keys)}");
            }
            return null;
        }
    }

    // 모든 이벤트 딕셔너리 반환
    public Dictionary<string, TrainEvent> GetAllTrainEvents()
    {
        return trainEventDic;
    }

    // 로드된 이벤트 수 반환
    public int GetEventCount()
    {
        return trainEventDic?.Count ?? 0;
    }

    public List<string> GetEntranceTrainEventID()
    {
        List<string> entranceEventIDList = new List<string>();
        foreach (var eventEntry in trainEventDic)
        {
            // "_ENTRANCE"로 끝나는 모든 이벤트 ID를 찾음
            if (eventEntry.Key.EndsWith("ENTRANCE"))
            {
                entranceEventIDList.Add(eventEntry.Key);
                Debug.Log($"Entrance 이벤트 찾음: {eventEntry.Key}");
            }
        }
        Debug.Log($"총 {entranceEventIDList.Count}개의 Entrance 이벤트 발견");
        return entranceEventIDList;
    }

    // 이벤트 존재 여부 확인
    public bool HasEvent(string eventID)
    {
        return trainEventDic != null && trainEventDic.ContainsKey(eventID);
    }

    public Sprite GetEventImage(string eventID)
    {
        return trainEventDic[eventID].GetEventImage();
    }
    
    // 이벤트 체인 검증 시스템
    private void ValidateEventChains()
    {
        Debug.Log("=== 이벤트 체인 검증 시작 ===");
        
        eventChainValidation = new Dictionary<string, bool>();
        unreachableEvents = new List<string>();
        invalidReferences = new List<string>();
        invalidRequirements = new List<string>();
        invalidResults = new List<string>();
        
        // 모든 이벤트에 대해 검증
        foreach (var eventEntry in trainEventDic)
        {
            string eventID = eventEntry.Key;
            TrainEvent trainEvent = eventEntry.Value;
            
            // 각 선택지의 참조 유효성 검사
            ValidateChoiceReference(eventID, trainEvent.GetEventChoice1(), 1);
            ValidateChoiceReference(eventID, trainEvent.GetEventChoice2(), 2);
            ValidateChoiceReference(eventID, trainEvent.GetEventChoice3(), 3);
            
            // 각 선택지의 요구사항 유효성 검사
            ValidateRequirement(eventID, trainEvent.GetEventChoice1Requirement(), 1);
            ValidateRequirement(eventID, trainEvent.GetEventChoice2Requirement(), 2);
            ValidateRequirement(eventID, trainEvent.GetEventChoice3Requirement(), 3);
            
            // 이벤트 결과 유효성 검사
            ValidateResult(eventID, trainEvent.GetEventResult());
        }
        
        // ENTRANCE 이벤트 그룹에서 도달 가능한 이벤트 검사
        HashSet<string> reachableEvents = new HashSet<string>();
        
        // ENTRANCE로 끝나는 모든 이벤트를 시작점으로 사용
        foreach (var eventID in trainEventDic.Keys)
        {
            if (eventID.EndsWith("ENTRANCE"))
            {
                TraverseEventChain(eventID, reachableEvents);
            }
        }
        
        // 도달 불가능한 이벤트 찾기
        foreach (var eventID in trainEventDic.Keys)
        {
            if (!reachableEvents.Contains(eventID) && !eventID.EndsWith("ENTRANCE"))
            {
                unreachableEvents.Add(eventID);
            }
        }
        
        // 검증 결과 보고
        ReportValidationResults();
        
        Debug.Log("=== 이벤트 체인 검증 완료 ===");
    }
    
    // 선택지 참조 유효성 검사
    private void ValidateChoiceReference(string eventID, string choiceReference, int choiceNumber)
    {
        if (string.IsNullOrEmpty(choiceReference) || choiceReference == "null")
        {
            return; // 유효한 null 참조
        }
        
        if (!trainEventDic.ContainsKey(choiceReference))
        {
            invalidReferences.Add($"{eventID} -> 선택지 {choiceNumber} -> {choiceReference}");
            Debug.LogWarning($"유효하지 않은 이벤트 참조: {eventID}의 선택지 {choiceNumber}이 존재하지 않는 이벤트 '{choiceReference}'를 참조합니다.");
        }
    }
    
    // 이벤트 체인 탐색
    private void TraverseEventChain(string eventID, HashSet<string> visited)
    {
        if (visited.Contains(eventID) || !trainEventDic.ContainsKey(eventID))
        {
            return;
        }
        
        visited.Add(eventID);
        TrainEvent trainEvent = trainEventDic[eventID];
        
        // 모든 선택지를 재귀적으로 탐색
        TraverseChoiceReference(trainEvent.GetEventChoice1(), visited);
        TraverseChoiceReference(trainEvent.GetEventChoice2(), visited);
        TraverseChoiceReference(trainEvent.GetEventChoice3(), visited);
    }
    
    // 선택지 참조 탐색
    private void TraverseChoiceReference(string choiceReference, HashSet<string> visited)
    {
        if (!string.IsNullOrEmpty(choiceReference) && choiceReference != "null")
        {
            TraverseEventChain(choiceReference, visited);
        }
    }
    
    // 요구사항 유효성 검사
    private void ValidateRequirement(string eventID, Dictionary<ResourceType, int> requirement, int choiceNumber)
    {
        if (requirement == null)
        {
            return;
        }
        
        foreach (var resource in requirement.Keys)
        {
            // 리소스 타입 검증
            if (!Enum.IsDefined(typeof(ResourceType), resource))
            {
                invalidRequirements.Add($"{eventID} -> 선택지 {choiceNumber} -> 유효하지 않은 리소스 타입: '{resource}'");
                Debug.LogWarning($"유효하지 않은 리소스 타입: {eventID}의 선택지 {choiceNumber}에서 '{resource}'");
            }
            else if (requirement[resource] == 0)
            {
                invalidRequirements.Add($"{eventID} -> 선택지 {choiceNumber} -> 유효하지 않은 수량: '{requirement[resource]}'");
                Debug.LogWarning($"유효하지 않은 수량: {eventID}의 선택지 {choiceNumber}에서 '{requirement[resource]}'");
            }
        }
    }
    
    // 결과 유효성 검사
    private void ValidateResult(string eventID, Dictionary<ResourceType, int> result)
    {
        if (result == null)
        {
            return;
        }
        
        foreach (var resource in result.Keys)
        {
            if (!Enum.IsDefined(typeof(ResourceType), resource))
            {
                invalidResults.Add($"{eventID} -> 유효하지 않은 리소스 타입: '{resource}'");
                Debug.LogWarning($"유효하지 않은 리소스 타입: {eventID}의 결과에서 '{resource}'");
            }
            
            // 수량 검증
            else if (result[resource] == 0)
            {
                invalidResults.Add($"{eventID} -> 유효하지 않은 수량: '{result[resource]}'");
                Debug.LogWarning($"유효하지 않은 수량: {eventID}의 결과에서 '{result[resource]}'");
            }
        }
    }
    
    // 유효한 리소스 타입인지 확인
    private bool IsValidResourceType(string resourceType)
    {
        // 게임에서 사용 가능한 리소스 타입 목록 (enum 값과 일치)
        string[] validResourceTypes = { "Food", "Fuel", "Passenger", "MachinePart", "Durability"};
        
        foreach (string validType in validResourceTypes)
        {
            if (resourceType.Equals(validType, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    // 검증 결과 보고
    private void ReportValidationResults()
    {
        Debug.Log($"=== 이벤트 체인 검증 결과 ===");
        Debug.Log($"총 이벤트 수: {trainEventDic.Count}");
        Debug.Log($"유효하지 않은 참조 수: {invalidReferences.Count}");
        Debug.Log($"유효하지 않은 요구사항 수: {invalidRequirements.Count}");
        Debug.Log($"유효하지 않은 결과 수: {invalidResults.Count}");
        Debug.Log($"도달 불가능한 이벤트 수: {unreachableEvents.Count}");
        
        if (invalidReferences.Count > 0)
        {
            Debug.LogWarning("유효하지 않은 참조들:");
            foreach (var invalidRef in invalidReferences)
            {
                Debug.LogWarning($"- {invalidRef}");
            }
        }
        
        if (invalidRequirements.Count > 0)
        {
            Debug.LogWarning("유효하지 않은 요구사항들:");
            foreach (var invalidReq in invalidRequirements)
            {
                Debug.LogWarning($"- {invalidReq}");
            }
        }
        
        if (invalidResults.Count > 0)
        {
            Debug.LogWarning("유효하지 않은 결과들:");
            foreach (var invalidRes in invalidResults)
            {
                Debug.LogWarning($"- {invalidRes}");
            }
        }
        
        if (unreachableEvents.Count > 0)
        {
            Debug.LogWarning("도달 불가능한 이벤트들:");
            foreach (var unreachableEvent in unreachableEvents)
            {
                Debug.LogWarning($"- {unreachableEvent}");
            }
        }
        
        if (invalidReferences.Count == 0 && invalidRequirements.Count == 0 && 
            invalidResults.Count == 0 && unreachableEvents.Count == 0)
        {
            Debug.Log("✅ 모든 이벤트 체인이 올바르게 구성되었습니다!");
        }
        else
        {
            int totalIssues = invalidReferences.Count + invalidRequirements.Count + 
                            invalidResults.Count + unreachableEvents.Count;
            Debug.LogWarning($"⚠️ 총 {totalIssues}개의 문제가 발견되었습니다.");
        }
    }
    
    // 이벤트 체인 검증 결과 조회 메서드
    public bool IsEventChainValid()
    {
        return invalidReferences.Count == 0 && invalidRequirements.Count == 0 && 
               invalidResults.Count == 0 && unreachableEvents.Count == 0;
    }
    
    // 유효하지 않은 참조 목록 반환
    public List<string> GetInvalidReferences()
    {
        return new List<string>(invalidReferences);
    }
    
    // 유효하지 않은 요구사항 목록 반환
    public List<string> GetInvalidRequirements()
    {
        return new List<string>(invalidRequirements);
    }
    
    // 유효하지 않은 결과 목록 반환
    public List<string> GetInvalidResults()
    {
        return new List<string>(invalidResults);
    }
    
    // 도달 불가능한 이벤트 목록 반환
    public List<string> GetUnreachableEvents()
    {
        return new List<string>(unreachableEvents);
    }
    
    // 이미지 사전 로딩 시스템
    private void PreloadAllEventImages()
    {
        Debug.Log("=== 이벤트 이미지 사전 로딩 시작 ===");
        
        if (trainEventDic == null || trainEventDic.Count == 0)
        {
            Debug.LogWarning("로드할 이벤트가 없습니다.");
            return;
        }
        
        // 비동기적으로 이미지 로딩 (코루틴 시뮬레이션)
        StartCoroutine(PreloadImagesCoroutine());
    }
    
    private System.Collections.IEnumerator PreloadImagesCoroutine()
    {
        int totalEvents = trainEventDic.Count;
        int processedEvents = 0;
        int successCount = 0;
        int failCount = 0;
        
        Debug.Log($"총 {totalEvents}개 이벤트의 이미지를 로딩합니다...");
        
        foreach (var eventEntry in trainEventDic)
        {
            string eventID = eventEntry.Key;
            TrainEvent trainEvent = eventEntry.Value;
            
            // 이미지가 이미 로드되어 있는지 확인
            if (trainEvent.GetEventImage() == null)
            {
                // 새로운 CSV Reader 인스턴스로 이미지 다시 로드 시도
                var tempReader = new TrainEventCSVReader();
                var loadedImage = LoadSingleEventImage(eventID);
                
                if (loadedImage != null)
                {
                    // TrainEvent 객체의 이미지 업데이트 (리플렉션 사용)
                    UpdateEventImage(trainEvent, loadedImage);
                    successCount++;
                    Debug.Log($"✅ [{processedEvents + 1}/{totalEvents}] {eventID} 이미지 로드 성공");
                }
                else
                {
                    failCount++;
                    Debug.LogWarning($"⚠️ [{processedEvents + 1}/{totalEvents}] {eventID} 이미지 로드 실패");
                }
            }
            else
            {
                successCount++;
                Debug.Log($"✅ [{processedEvents + 1}/{totalEvents}] {eventID} 이미지 이미 로드됨");
            }
            
            processedEvents++;
            
            // 프레임 드랍 방지를 위해 몇 개마다 한 프레임 대기
            if (processedEvents % 3 == 0)
            {
                yield return null;
            }
        }
        
        Debug.Log($"=== 이미지 사전 로딩 완료 ===");
        Debug.Log($"총 {totalEvents}개 중 성공: {successCount}개, 실패: {failCount}개");
        Debug.Log($"성공률: {(float)successCount / totalEvents * 100:F1}%");
        
        // 캐시 크기 보고
        Debug.Log($"이미지 캐시 크기: {TrainEventCSVReader.GetCacheSize()}개");
    }
    
    private Sprite LoadSingleEventImage(string eventID)
    {
        if (string.IsNullOrEmpty(eventID))
            return null;
            
        // 여러 경로에서 이미지 찾기 시도
        string[] possiblePaths = {
            $"Sprites/EventImages/{eventID}",
            $"Sprites/EventImages/{eventID.ToLower()}",
            $"Sprites/EventImages/{eventID.ToUpper()}",
        };
        
        foreach (string path in possiblePaths)
        {
            Sprite sprite = Resources.Load<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }
        }
        
        return null;
    }
    
    private void UpdateEventImage(TrainEvent trainEvent, Sprite newImage)
    {
        // TrainEvent의 SetEventImage 메서드 사용
        if (trainEvent != null)
        {
            trainEvent.SetEventImage(newImage);
        }
    }
    
    // 런타임에 특정 이벤트의 이미지를 다시 로드하는 메서드
    public bool ReloadEventImage(string eventID)
    {
        if (string.IsNullOrEmpty(eventID) || !trainEventDic.ContainsKey(eventID))
        {
            Debug.LogWarning($"유효하지 않은 이벤트 ID: {eventID}");
            return false;
        }
        
        Sprite newImage = LoadSingleEventImage(eventID);
        if (newImage != null)
        {
            TrainEvent trainEvent = trainEventDic[eventID];
            UpdateEventImage(trainEvent, newImage);
            Debug.Log($"✅ {eventID} 이미지 다시 로드 성공");
            return true;
        }
        else
        {
            Debug.LogWarning($"⚠️ {eventID} 이미지 다시 로드 실패");
            return false;
        }
    }
    
    // 이미지 캐시 관리 메서드들
    public void ClearImageCache()
    {
        TrainEventCSVReader.ClearImageCache();
    }
    
    public int GetImageCacheSize()
    {
        return TrainEventCSVReader.GetCacheSize();
    }

    // 이미지 사전 로딩을 지연 실행하는 코루틴
    private System.Collections.IEnumerator DelayedImagePreload()
    {
        // 한 프레임 대기 후 이미지 로딩 시작
        yield return null;
        PreloadAllEventImages();
    }
}