using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TrainEventUIManager : UIManager
{
    [Header("UI 요소들")]
    public GameObject eventPanel;
    public Image eventImage;
    public TextMeshProUGUI eventDescription;
    public Button[] choiceButtons = new Button[3];
    public TextMeshProUGUI[] choiceTexts = new TextMeshProUGUI[3];
    
    [Header("설정")]
    public Sprite defaultEventImage; // Inspector에서 할당할 기본 이미지
    
    private TrainEventManager trainEventManager;
    private Coroutine autoCloseCoroutine;
    private string currentEventID = "";

    protected override void Start()
    {   
        base.Start(); // UIManager의 Start 호출
        
        // 싱글톤 패턴을 사용하여 TrainEventManager 참조
        trainEventManager = TrainEventManager.Instance;
        
        if (trainEventManager == null)
        {
            Debug.LogError("TrainEventManager를 찾을 수 없습니다!");
            return;
        }
        
        // 이벤트 로딩 확인
        if (trainEventManager.GetEventCount() == 0)
        {
            Debug.LogWarning("이벤트가 로드되지 않았습니다!");
        }
        
        // eventPanel 확인
        if (eventPanel == null)
        {
            Debug.LogError("eventPanel이 할당되지 않았습니다!");
        }
        
        CloseEvent();
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy(); // UIManager의 OnDestroy 호출
        
        // 메모리 누수 방지를 위한 정리
        StopAllAnimations();
        
        // 버튼 이벤트 리스너 정리
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null) choiceButtons[i].onClick.RemoveAllListeners();
        }
    }
    
    // UIManager의 OnCloseButtonClick 오버라이드
    protected override void OnCloseButtonClick()
    {
        CloseEvent();
    }
    
    public void ShowEvent(string eventID)
    {
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
        
        currentEventID = eventID;
        
        if (eventPanel != null)
        {
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

        if (eventPanel != null)
        {
            // 페이드 아웃 애니메이션과 함께 패널 닫기
            fadeCoroutine = StartCoroutine(CloseEventWithAnimation());
        }
    }
    
    // 선택지 클릭 이벤트 (통합)
    public void OnChoiceClick(string nextEventID, int choiceNumber)
    {

        
        // 타이핑 애니메이션이 활성화되고 진행 중일 때만 클릭 무시
        if (enableTypingAnimation && isTypingAnimation)
        {

            return;
        }
        
        if (string.IsNullOrEmpty(nextEventID) || nextEventID == "null")
        {
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

            autoCloseCoroutine = StartCoroutine(AutoCloseEventAfterDelay(3.0f));
        }
        else
        {

        }
    }
    
    // 지연 후 이벤트 자동 닫기 코루틴
    private IEnumerator AutoCloseEventAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
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
            string[] choiceNextEventIDs = { trainEvent.GetEventChoice1(), trainEvent.GetEventChoice2(), trainEvent.GetEventChoice3() };
            string[] choiceDescriptions = { trainEvent.GetEventChoice1Description(), trainEvent.GetEventChoice2Description(), trainEvent.GetEventChoice3Description() };
            
            for (int i = 0; i < 3; i++)
            {
                SetupChoice(choiceButtons[i], choiceTexts[i], choiceNextEventIDs[i], choiceDescriptions[i], i + 1);
            }
            
    
            
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
            // 이미지가 있는 경우
            eventImage.sprite = trainEvent.GetEventImage();
        }
        else
        {
            // 이미지가 없는 경우 기본 이미지 사용
            ApplyFallbackImage();
        }
    }
    
    // 기본 이미지 적용
    private void ApplyFallbackImage()
    {
        eventImage.sprite = defaultEventImage; // null이어도 상관없음
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
            SetButtonActive(button, true);
            
            // 기존 리스너 제거 후 새 리스너 추가
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnChoiceClick(nextEventID, choiceNumber));
            

        }
        else
        {
            Debug.LogWarning($"선택지 {choiceNumber}가 없습니다.");
            SetButtonActive(button, false);
        }
    }

     // 애니메이션 관련 메서드들
    private IEnumerator ShowEventWithAnimation(TrainEvent trainEvent)
    {
         // 패널 활성화
        eventPanel.SetActive(true);
        
         // 초기 알파값 설정 (완전 투명)
        CanvasGroup canvasGroup = eventPanel.GetComponent<CanvasGroup>();
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
    
            }
        }
        
         // 선택지가 없는 이벤트인지 확인하고 자동 닫기
        CheckAndAutoCloseEvent(trainEvent);
    }
    
        private IEnumerator CloseEventWithAnimation()
    {
        CanvasGroup canvasGroup = eventPanel.GetComponent<CanvasGroup>();
        yield return StartCoroutine(FadeOut(canvasGroup));
        
        eventPanel.SetActive(false);
    }
    
        protected override void StopAllAnimations()
    {
        base.StopAllAnimations();
        
        // TrainEventUIManager 전용 코루틴 중지
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = null;
        }
        
        // 나머지 모든 코루틴도 중지 (안전장치)
        StopAllCoroutines();
        

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
        string[] choiceNextEventIDs = { trainEvent.GetEventChoice1(), trainEvent.GetEventChoice2(), trainEvent.GetEventChoice3() };
        string[] choiceDescriptions = { trainEvent.GetEventChoice1Description(), trainEvent.GetEventChoice2Description(), trainEvent.GetEventChoice3Description() };
        
        for (int i = 0; i < 3; i++)
        {
            SetupChoice(choiceButtons[i], choiceTexts[i], choiceNextEventIDs[i], choiceDescriptions[i], i + 1);
        }
        

    }

}
