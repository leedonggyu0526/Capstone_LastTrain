using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TrainEventUIManager : MonoBehaviour
{
    [Header("UI 요소들")]
    public GameObject eventPanel;
    public Image eventImage;
    public TextMeshProUGUI eventDescription;
    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;
    public TextMeshProUGUI choice3Text;
    
    [Header("설정")]
    public Sprite defaultEventImage; // Inspector에서 할당할 기본 이미지
    public Sprite loadingImage; // 이미지 로딩 중에 표시할 이미지
    [Range(0.1f, 2f)]
    public float typingSpeed = 0.05f; // 타이핑 애니메이션 속도
    [Range(0.1f, 1f)]
    public float fadeSpeed = 0.5f; // 페이드 애니메이션 속도
    public bool enableDynamicImageLoading = true; // 동적 이미지 로딩 활성화
    public bool enableTypingAnimation = true; // 타이핑 애니메이션 활성화
    
    private TrainEventManager trainEventManager;
    private bool isInitialized = false;
    private bool isTypingAnimation = false;
    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;
    private Coroutine autoCloseCoroutine;
    
    // 선택 히스토리 추적
    private List<EventChoice> choiceHistory = new List<EventChoice>();
    private string currentEventID = "";
    
    // 이벤트 선택 정보 저장용 구조체
    [System.Serializable]
    public struct EventChoice
    {
        public string eventID;
        public string eventName;
        public int choiceNumber;
        public string choiceDescription;
        public string nextEventID;
        public System.DateTime timestamp;
        
        public EventChoice(string eventID, string eventName, int choiceNumber, string choiceDescription, string nextEventID)
        {
            this.eventID = eventID;
            this.eventName = eventName;
            this.choiceNumber = choiceNumber;
            this.choiceDescription = choiceDescription;
            this.nextEventID = nextEventID;
            this.timestamp = System.DateTime.Now;
        }
    }

    void Start()
    {   
        Debug.Log("이벤트 UI 매니저 초기화");
        
        // 싱글톤 패턴을 사용하여 TrainEventManager 참조
        trainEventManager = TrainEventManager.Instance;
        
        if (trainEventManager == null)
        {
            Debug.LogError("TrainEventManager를 찾을 수 없습니다! 씬에 TrainEventManager 컴포넌트가 있는지 확인해주세요.");
            return;
        }
        
        Debug.Log("TrainEventManager 찾기 성공!");
        Debug.Log($"로드된 이벤트 수: {trainEventManager.GetEventCount()}");
        
        // 안전한 이벤트 정보 출력 - 먼저 사용 가능한 이벤트가 있는지 확인
        if (trainEventManager.GetEventCount() > 0)
        {
            // 'START' 이벤트를 시도해보고, 없으면 첫 번째 이벤트 사용
            TrainEvent testEvent = trainEventManager.GetTrainEvent("START");
            if (testEvent != null)
            {
                Debug.Log("'START' 이벤트 정보:");
                Debug.Log(testEvent.GetAllEventInfo());
            }
            else
            {
                Debug.LogWarning("이벤트 ID 'START'를 찾을 수 없습니다. 첫 번째 이벤트를 시도합니다.");
                
                // 첫 번째 이벤트 가져오기
                var allEvents = trainEventManager.GetAllTrainEvents();
                if (allEvents != null && allEvents.Count > 0)
                {
                    foreach (var kvp in allEvents)
                    {
                        Debug.Log($"첫 번째 이벤트 (ID: {kvp.Key}) 정보:");
                        Debug.Log(kvp.Value.GetAllEventInfo());
                        break; // 첫 번째만 출력
                    }
                }
            }
            isInitialized = true;
        }
        else
        {
            Debug.LogWarning("로드된 이벤트가 없습니다! CSV 파일을 확인해주세요.");
        }
        
        // eventPanel 상태 확인
        if (eventPanel == null)
        {
            Debug.LogError("eventPanel이 할당되지 않았습니다! Inspector에서 할당해주세요.");
        }
        else
        {
            Debug.Log($"eventPanel 상태: {eventPanel.name}, 활성화됨: {eventPanel.activeInHierarchy}");
        }
        
        CloseEvent();
        Debug.Log("TrainEventUIManager 초기화 완료");
    }
    
    void OnDestroy()
    {
        // 메모리 누수 방지를 위한 정리
        StopAllAnimations();
        
        // 버튼 이벤트 리스너 정리
        if (choice1Button != null) choice1Button.onClick.RemoveAllListeners();
        if (choice2Button != null) choice2Button.onClick.RemoveAllListeners();
        if (choice3Button != null) choice3Button.onClick.RemoveAllListeners();
        
        Debug.Log("TrainEventUIManager 정리 완료");
    }

    void Update()
    {
        // 스페이스 키 입력 확인
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== 스페이스 키 입력 감지됨 ===");
            
            // 초기화 상태 확인
            if (!isInitialized)
            {
                Debug.LogWarning("TrainEventUIManager가 아직 초기화되지 않았습니다!");
                return;
            }
            
            // TrainEventManager 상태 확인
            if (trainEventManager == null)
            {
                Debug.LogError("trainEventManager가 null입니다!");
                return;
            }
            
            // 이벤트 개수 확인
            int eventCount = trainEventManager.GetEventCount();
            Debug.Log($"현재 이벤트 개수: {eventCount}");
            
            if (eventCount <= 0)
            {
                Debug.LogWarning("표시할 이벤트가 없습니다!");
                return;
            }
            
            // 'START' 이벤트가 있는지 확인
            if (trainEventManager.HasEvent("START"))
            {
                Debug.Log("'START' 이벤트를 표시합니다.");
                ShowEvent("START");
            }
            else
            {
                Debug.Log("'START' 이벤트가 없어서 첫 번째 이벤트를 표시합니다.");
                var allEvents = trainEventManager.GetAllTrainEvents();
                if (allEvents != null && allEvents.Count > 0)
                {
                    foreach (var kvp in allEvents)
                    {
                        Debug.Log($"첫 번째 이벤트 표시: {kvp.Key}");
                        ShowEvent(kvp.Key);
                        break;
                    }
                }
            }
            Debug.Log("=== 스페이스 키 처리 완료 ===");
        }
    }
    
    public void ShowEvent(string eventID)
    {
        Debug.Log($"ShowEvent 호출됨: {eventID}");
        
        if (trainEventManager == null)
        {
            Debug.LogError("TrainEventManager가 null입니다!");
            return;
        }
        
        TrainEvent trainEvent = trainEventManager.GetTrainEvent(eventID);
        if (trainEvent == null)
        {
            Debug.LogWarning($"이벤트 ID '{eventID}'를 찾을 수 없습니다!");
            return;
        }
        
        Debug.Log("이벤트 표시: " + trainEvent.GetEventName());
        currentEventID = eventID;
        
        if (eventPanel != null)
        {
            Debug.Log($"eventPanel을 활성화합니다. 현재 상태: {eventPanel.activeSelf}");
            
            // 기존 애니메이션 중지
            StopAllAnimations();
            
            // 페이드 인 애니메이션과 함께 패널 활성화
            fadeCoroutine = StartCoroutine(ShowEventWithAnimation(trainEvent));
        }
        else
        {
            Debug.LogWarning("eventPanel이 할당되지 않았습니다!");
        }
    }

    public void CloseEvent()
    {
        Debug.Log("이벤트 닫기");
        if (eventPanel != null)
        {
            // 페이드 아웃 애니메이션과 함께 패널 닫기
            fadeCoroutine = StartCoroutine(CloseEventWithAnimation());
        }
    }
    
    // 선택지 클릭 이벤트 (통합)
    public void OnChoiceClick(string nextEventID, int choiceNumber)
    {
        Debug.Log($"선택지 {choiceNumber} 클릭됨 - 다음 이벤트: {nextEventID}");
        
        // 타이핑 애니메이션이 활성화되고 진행 중일 때만 클릭 무시
        if (enableTypingAnimation && isTypingAnimation)
        {
            Debug.Log("타이핑 애니메이션 중에는 선택지 클릭이 무시됩니다.");
            return;
        }
        
        // 선택 히스토리에 추가
        TrainEvent currentEvent = trainEventManager.GetTrainEvent(currentEventID);
        if (currentEvent != null)
        {
            string choiceDescription = "";
            switch (choiceNumber)
            {
                case 1: choiceDescription = currentEvent.GetEventChoice1Description(); break;
                case 2: choiceDescription = currentEvent.GetEventChoice2Description(); break;
                case 3: choiceDescription = currentEvent.GetEventChoice3Description(); break;
            }
            
            EventChoice choice = new EventChoice(currentEventID, currentEvent.GetEventName(), choiceNumber, choiceDescription, nextEventID);
            choiceHistory.Add(choice);
            
            Debug.Log($"선택 히스토리에 추가됨: {currentEvent.GetEventName()} -> {choiceDescription}");
        }
        
        if (string.IsNullOrEmpty(nextEventID) || nextEventID == "null")
        {
            Debug.Log("더 이상 진행할 이벤트가 없습니다. 이벤트를 닫습니다.");
            ShowChoiceHistory(); // 히스토리 출력
            CloseEvent();
            return;
        }
        
        if(trainEventManager.HasEvent(nextEventID)){
            ShowEvent(nextEventID);
        }else{
            Debug.LogWarning($"이벤트 ID '{nextEventID}'를 찾을 수 없습니다!");
            CloseEvent();
            return;
        }
    }
    
    // 선택지가 없는 이벤트인지 확인하고 자동 닫기
    private void CheckAndAutoCloseEvent(TrainEvent trainEvent)
    {
        // 모든 선택지가 없거나 null인지 확인
        bool hasChoice1 = !string.IsNullOrEmpty(trainEvent.GetEventChoice1()) && trainEvent.GetEventChoice1() != "null";
        bool hasChoice2 = !string.IsNullOrEmpty(trainEvent.GetEventChoice2()) && trainEvent.GetEventChoice2() != "null";
        bool hasChoice3 = !string.IsNullOrEmpty(trainEvent.GetEventChoice3()) && trainEvent.GetEventChoice3() != "null";
        
        bool hasAnyChoice = hasChoice1 || hasChoice2 || hasChoice3;
        
        if (!hasAnyChoice)
        {
            Debug.Log("선택지가 없는 이벤트입니다. 3초 후 자동으로 닫힙니다.");
            autoCloseCoroutine = StartCoroutine(AutoCloseEventAfterDelay(3.0f));
        }
        else
        {
            Debug.Log("선택지가 있는 이벤트입니다. 자동으로 닫히지 않습니다.");
        }
    }
    
    // 지연 후 이벤트 자동 닫기 코루틴
    private IEnumerator AutoCloseEventAfterDelay(float delay)
    {
        Debug.Log($"{delay}초 대기 중...");
        yield return new WaitForSeconds(delay);
        
        Debug.Log("대기 완료, 이벤트를 닫습니다.");
        CloseEvent();
    }
    
    // UI 요소들을 안전하게 설정하는 메서드
    private void SetupEventUI(TrainEvent trainEvent)
    {
        try
        {
            // 이벤트 이미지 설정
            SetupEventImage(trainEvent);
            
            // 이벤트 설명 설정
            SetupEventDescription(trainEvent);
            
            // 선택지들 설정
            SetupChoice(choice1Button, choice1Text, trainEvent.GetEventChoice1(), trainEvent.GetEventChoice1Description(), 1);
            SetupChoice(choice2Button, choice2Text, trainEvent.GetEventChoice2(), trainEvent.GetEventChoice2Description(), 2);
            SetupChoice(choice3Button, choice3Text, trainEvent.GetEventChoice3(), trainEvent.GetEventChoice3Description(), 3);
            
            Debug.Log("UI 요소들이 성공적으로 설정되었습니다.");
            
            // 선택지가 없는 이벤트인지 확인하고 자동 닫기
            CheckAndAutoCloseEvent(trainEvent);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UI 요소 설정 중 오류 발생: {e.Message}");
            Debug.LogError("UI 요소들이 올바르게 할당되었는지 확인해주세요.");
        }
    }
    
    // 이벤트 이미지 설정 (개선된 버전)
    private void SetupEventImage(TrainEvent trainEvent)
    {
        if (eventImage == null)
        {
            Debug.LogWarning("eventImage가 할당되지 않았습니다.");
            return;
        }
        
        // 이미지 색상 초기화 (투명도 해제)
        eventImage.color = Color.white;
        
        if (trainEvent.GetEventImage() != null)
        {
            // 이미지가 이미 로드되어 있는 경우
            eventImage.sprite = trainEvent.GetEventImage();
            Debug.Log($"✅ 이벤트 이미지 설정 완료: {currentEventID}");
        }
        else if (enableDynamicImageLoading)
        {
            // 동적 이미지 로딩 시도
            Debug.Log($"🔄 동적 이미지 로딩 시작: {currentEventID}");
            StartCoroutine(LoadEventImageDynamically(trainEvent, currentEventID));
        }
        else
        {
            // 동적 로딩이 비활성화된 경우 기본 이미지 사용
            ApplyFallbackImage();
        }
    }
    
    // 동적 이미지 로딩 코루틴
    private IEnumerator LoadEventImageDynamically(TrainEvent trainEvent, string eventID)
    {
        // 로딩 이미지 표시
        if (loadingImage != null)
        {
            eventImage.sprite = loadingImage;
            Debug.Log($"📱 로딩 이미지 표시: {eventID}");
        }
        
        // 한 프레임 대기 (UI 업데이트를 위해)
        yield return null;
        
        // TrainEventManager에서 이미지 다시 로드 시도
        bool reloadSuccess = false;
        
        if (trainEventManager != null)
        {
            reloadSuccess = trainEventManager.ReloadEventImage(eventID);
        }
        
        // 로딩 완료 후 UI 업데이트
        if (reloadSuccess && trainEvent.GetEventImage() != null)
        {
            // 로딩 성공
            eventImage.sprite = trainEvent.GetEventImage();
            Debug.Log($"✅ 동적 이미지 로딩 성공: {eventID}");
            
            // 이미지 페이드인 효과
            if (eventImage.GetComponent<CanvasGroup>() == null)
            {
                eventImage.gameObject.AddComponent<CanvasGroup>();
            }
            StartCoroutine(FadeInImage(eventImage.GetComponent<CanvasGroup>()));
        }
        else
        {
            // 로딩 실패 - 기본 이미지 사용
            Debug.LogWarning($"⚠️ 동적 이미지 로딩 실패: {eventID}");
            ApplyFallbackImage();
        }
    }
    
    // 대체 이미지 적용
    private void ApplyFallbackImage()
    {
        if (defaultEventImage != null)
        {
            eventImage.sprite = defaultEventImage;
            Debug.Log("📋 기본 이미지를 사용합니다.");
        }
        else
        {
            // 이미지가 전혀 없는 경우 반투명으로 표시
            eventImage.sprite = null;
            eventImage.color = new Color(1f, 1f, 1f, 0.3f);
            Debug.LogWarning("⚠️ 표시할 이미지가 없습니다. 반투명 처리됩니다.");
        }
    }
    
    // 이미지 페이드인 효과
    private IEnumerator FadeInImage(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        float duration = 0.5f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    // 이벤트 설명 설정
    private void SetupEventDescription(TrainEvent trainEvent)
    {
        if (eventDescription == null)
        {
            Debug.LogWarning("eventDescription이 할당되지 않았습니다.");
            return;
        }
        
        eventDescription.text = trainEvent.GetEventDescription();
        Debug.Log($"이벤트 설명 설정: {trainEvent.GetEventDescription()}");
    }
    
    // 선택지 설정
    private void SetupChoice(Button button, TextMeshProUGUI text, string nextEventID, string choiceDescription, int choiceNumber)
    {
        if (button == null || text == null)
        {
            Debug.LogWarning($"선택지 {choiceNumber}의 Button 또는 Text가 할당되지 않았습니다.");
            return;
        }
        
        // 선택지가 유효한지 확인
        bool hasValidChoice = !string.IsNullOrEmpty(nextEventID) && nextEventID != "null" && 
                              !string.IsNullOrEmpty(choiceDescription) && choiceDescription != "null";
        
        if (hasValidChoice)
        {
            text.text = choiceDescription;
            button.gameObject.SetActive(true);
            
            // 기존 리스너 제거 후 새 리스너 추가
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnChoiceClick(nextEventID, choiceNumber));
            
            Debug.Log($"선택지 {choiceNumber} 설정: {choiceDescription} -> {nextEventID}");
        }
        else
        {
            Debug.LogWarning($"선택지 {choiceNumber}가 없습니다.");
                         button.gameObject.SetActive(false);
         }
     }
     
     // 애니메이션 관련 메서드들
     private IEnumerator ShowEventWithAnimation(TrainEvent trainEvent)
     {
         // 패널 활성화
         eventPanel.SetActive(true);
         
         // 초기 알파값 설정 (완전 투명)
         CanvasGroup canvasGroup = eventPanel.GetComponent<CanvasGroup>();
         if (canvasGroup == null)
         {
             canvasGroup = eventPanel.AddComponent<CanvasGroup>();
         }
         canvasGroup.alpha = 0f;
         
         // 기본 UI 요소 설정 (모든 요소를 처음부터 설정)
         SetupEventUIComplete(trainEvent);
         
         // 페이드 인 애니메이션
         yield return StartCoroutine(FadeIn(canvasGroup));
         
         // 설명 텍스트 설정 (타이핑 애니메이션 또는 즉시 표시)
         if (eventDescription != null)
         {
             if (enableTypingAnimation)
             {
                 // 타이핑 애니메이션으로 표시
                 typingCoroutine = StartCoroutine(TypeText(eventDescription, trainEvent.GetEventDescription()));
                 yield return typingCoroutine;
             }
             else
             {
                 // 즉시 모든 텍스트 표시
                 eventDescription.text = trainEvent.GetEventDescription();
                 Debug.Log("텍스트 즉시 표시 완료");
             }
         }
         
         // 선택지가 없는 이벤트인지 확인하고 자동 닫기
         CheckAndAutoCloseEvent(trainEvent);
     }
     
     private IEnumerator CloseEventWithAnimation()
     {
         CanvasGroup canvasGroup = eventPanel.GetComponent<CanvasGroup>();
         if (canvasGroup != null)
         {
             yield return StartCoroutine(FadeOut(canvasGroup));
         }
         
         eventPanel.SetActive(false);
     }
     
     private IEnumerator FadeIn(CanvasGroup canvasGroup)
     {
         float elapsedTime = 0f;
         float duration = 1f / fadeSpeed;
         
         while (elapsedTime < duration)
         {
             elapsedTime += Time.deltaTime;
             canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
             yield return null;
         }
         
         canvasGroup.alpha = 1f;
     }
     
     private IEnumerator FadeOut(CanvasGroup canvasGroup)
     {
         float elapsedTime = 0f;
         float duration = 1f / fadeSpeed;
         
         while (elapsedTime < duration)
         {
             elapsedTime += Time.deltaTime;
             canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
             yield return null;
         }
         
         canvasGroup.alpha = 0f;
     }
     
     private IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
     {
         isTypingAnimation = true;
         textComponent.text = "";
         
         for (int i = 0; i <= fullText.Length; i++)
         {
             textComponent.text = fullText.Substring(0, i);
             yield return new WaitForSeconds(typingSpeed);
         }
         
         isTypingAnimation = false;
     }
     
     // 더 이상 사용하지 않는 메서드 (순차적 선택지 표시)
     // 이제 모든 선택지가 처음부터 함께 표시됩니다
     /*
     private IEnumerator ShowChoicesSequentially(TrainEvent trainEvent)
     {
         // 모든 선택지 버튼을 먼저 숨김
         choice1Button?.gameObject.SetActive(false);
         choice2Button?.gameObject.SetActive(false);
         choice3Button?.gameObject.SetActive(false);
         
         yield return new WaitForSeconds(0.3f);
         
         // 선택지들을 순차적으로 표시
         if (trainEvent.HasChoice1() && choice1Button != null)
         {
             choice1Button.gameObject.SetActive(true);
             yield return new WaitForSeconds(0.2f);
         }
         
         if (trainEvent.HasChoice2() && choice2Button != null)
         {
             choice2Button.gameObject.SetActive(true);
             yield return new WaitForSeconds(0.2f);
         }
         
         if (trainEvent.HasChoice3() && choice3Button != null)
         {
             choice3Button.gameObject.SetActive(true);
             yield return new WaitForSeconds(0.2f);
         }
     }
     */
     
     private void StopAllAnimations()
     {
         // 개별 코루틴 중지
         if (typingCoroutine != null)
         {
             StopCoroutine(typingCoroutine);
             typingCoroutine = null;
         }
         
         if (fadeCoroutine != null)
         {
             StopCoroutine(fadeCoroutine);
             fadeCoroutine = null;
         }
         
         if (autoCloseCoroutine != null)
         {
             StopCoroutine(autoCloseCoroutine);
             autoCloseCoroutine = null;
         }
         
         // 나머지 모든 코루틴도 중지 (안전장치)
         StopAllCoroutines();
         
         // 애니메이션 상태 초기화
         isTypingAnimation = false;
         
         Debug.Log("모든 애니메이션이 중지되었습니다.");
     }
     
     // 모든 UI 요소를 처음부터 완전히 설정하는 메서드
     private void SetupEventUIComplete(TrainEvent trainEvent)
     {
         // 이미지 설정
         SetupEventImage(trainEvent);
         
         // 설명 텍스트는 비우기 (타이핑 애니메이션을 위해)
         if (eventDescription != null)
         {
             eventDescription.text = "";
         }
         
         // 선택지 설정 (모든 선택지를 즉시 표시)
         SetupChoice(choice1Button, choice1Text, trainEvent.GetEventChoice1(), trainEvent.GetEventChoice1Description(), 1);
         SetupChoice(choice2Button, choice2Text, trainEvent.GetEventChoice2(), trainEvent.GetEventChoice2Description(), 2);
         SetupChoice(choice3Button, choice3Text, trainEvent.GetEventChoice3(), trainEvent.GetEventChoice3Description(), 3);
         
         Debug.Log("모든 UI 요소가 즉시 설정되었습니다 (선택지 포함)");
     }
     
     // 기존 메서드도 유지 (호환성을 위해)
     private void SetupEventUIBase(TrainEvent trainEvent)
     {
         // 이미지 설정
         SetupEventImage(trainEvent);
         
         // 설명 텍스트는 비우기 (타이핑 애니메이션을 위해)
         if (eventDescription != null)
         {
             eventDescription.text = "";
         }
         
         // 선택지 설정 (텍스트만, 나중에 순차적으로 표시)
         SetupChoice(choice1Button, choice1Text, trainEvent.GetEventChoice1(), trainEvent.GetEventChoice1Description(), 1);
         SetupChoice(choice2Button, choice2Text, trainEvent.GetEventChoice2(), trainEvent.GetEventChoice2Description(), 2);
         SetupChoice(choice3Button, choice3Text, trainEvent.GetEventChoice3(), trainEvent.GetEventChoice3Description(), 3);
     }
     
     private void ShowChoiceHistory()
     {
         Debug.Log("=== 선택 히스토리 ===");
         for (int i = 0; i < choiceHistory.Count; i++)
         {
             EventChoice choice = choiceHistory[i];
             Debug.Log($"{i + 1}. [{choice.timestamp:HH:mm:ss}] {choice.eventName} -> {choice.choiceDescription}");
         }
         Debug.Log($"총 {choiceHistory.Count}개의 선택을 했습니다.");
     }
     
     // 히스토리 관련 public 메서드들
     public List<EventChoice> GetChoiceHistory()
     {
         return new List<EventChoice>(choiceHistory);
     }
     
     public void ClearChoiceHistory()
     {
         choiceHistory.Clear();
         Debug.Log("선택 히스토리가 초기화되었습니다.");
     }
     
     public int GetChoiceHistoryCount()
     {
         return choiceHistory.Count;
     }
     
     // 이미지 관리 및 디버깅 메서드들
     [System.Obsolete("디버깅용 메서드입니다. Inspector에서 호출하지 마세요.")]
     public void DebugImageCache()
     {
         if (trainEventManager != null)
         {
             Debug.Log($"=== 이미지 캐시 상태 ===");
             Debug.Log($"캐시 크기: {trainEventManager.GetImageCacheSize()}개");
             Debug.Log($"현재 이벤트: {currentEventID}");
             
             TrainEvent currentEvent = trainEventManager.GetTrainEvent(currentEventID);
             if (currentEvent != null)
             {
                 bool hasImage = currentEvent.GetEventImage() != null;
                 Debug.Log($"현재 이벤트 이미지 상태: {(hasImage ? "로드됨" : "로드되지 않음")}");
             }
         }
     }
     
     [System.Obsolete("디버깅용 메서드입니다. Inspector에서 호출하지 마세요.")]
     public void ClearImageCache()
     {
         if (trainEventManager != null)
         {
             trainEventManager.ClearImageCache();
             Debug.Log("이미지 캐시가 초기화되었습니다.");
         }
     }
     
     [System.Obsolete("디버깅용 메서드입니다. Inspector에서 호출하지 마세요.")]
     public void ReloadCurrentEventImage()
     {
         if (!string.IsNullOrEmpty(currentEventID) && trainEventManager != null)
         {
             Debug.Log($"현재 이벤트 이미지 다시 로딩: {currentEventID}");
             bool success = trainEventManager.ReloadEventImage(currentEventID);
             
             if (success)
             {
                 // UI 이미지 업데이트
                 TrainEvent currentEvent = trainEventManager.GetTrainEvent(currentEventID);
                 if (currentEvent != null && eventImage != null)
                 {
                     eventImage.sprite = currentEvent.GetEventImage();
                     eventImage.color = Color.white;
                     Debug.Log("✅ UI 이미지 업데이트 완료");
                 }
             }
         }
         else
         {
             Debug.LogWarning("현재 이벤트가 없거나 TrainEventManager가 null입니다.");
         }
     }
     
     // 이미지 설정 유틸리티 메서드들
     public void SetDefaultImage(Sprite newDefaultImage)
     {
         defaultEventImage = newDefaultImage;
         Debug.Log("기본 이미지가 변경되었습니다.");
     }
     
     public void SetLoadingImage(Sprite newLoadingImage)
     {
         loadingImage = newLoadingImage;
         Debug.Log("로딩 이미지가 변경되었습니다.");
     }
     
     public void ToggleDynamicImageLoading()
     {
         enableDynamicImageLoading = !enableDynamicImageLoading;
         Debug.Log($"동적 이미지 로딩: {(enableDynamicImageLoading ? "활성화" : "비활성화")}");
     }
     
     public void ToggleTypingAnimation()
     {
         enableTypingAnimation = !enableTypingAnimation;
         Debug.Log($"타이핑 애니메이션: {(enableTypingAnimation ? "활성화" : "비활성화")}");
     }
     
     // 모든 애니메이션 설정 토글
     public void ToggleAllAnimations()
     {
         enableTypingAnimation = !enableTypingAnimation;
         Debug.Log($"모든 애니메이션: {(enableTypingAnimation ? "활성화" : "비활성화")}");
     }
}
