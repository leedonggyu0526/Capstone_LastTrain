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
    public GameObject[] eventChoiceRequirements = new GameObject[3];
    public Sprite fuelItemImage;
    public Sprite foodItemImage;
    public Sprite machinePartItemImage;
    public Sprite passengerItemImage;
    private Dictionary<ResourceType, Sprite> requirementObjectImages = new Dictionary<ResourceType, Sprite>();

    [Header("설정")]
    public Sprite defaultEventImage; // Inspector에서 할당할 기본 이미지
    
    private TrainEventManager trainEventManager;
    private string currentEventID = "";
    protected override void Start()
    {   
        base.Start(); // UIManager의 Start 호출
        
        // UI 요소 검증
        ValidateUIElements();
        
        // 싱글톤 패턴을 사용하여 TrainEventManager 참조
        trainEventManager = TrainEventManager.Instance;

        requirementObjectImages.Add(ResourceType.Fuel, fuelItemImage);
        requirementObjectImages.Add(ResourceType.Food, foodItemImage);
        requirementObjectImages.Add(ResourceType.MachinePart, machinePartItemImage);
        requirementObjectImages.Add(ResourceType.Passenger, passengerItemImage);
        
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
        
        // eventPanel 확인 및 초기 설정
        if (eventPanel == null)
        {
            Debug.LogError("eventPanel이 할당되지 않았습니다!");
        }
        else
        {
            // 시작 시 패널 비활성화 (애니메이션 없이)
            CanvasGroup canvasGroup = eventPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f; // 알파값은 1로 유지
            }
            eventPanel.SetActive(false);
        }
    }
    
    // UI 요소들이 올바르게 할당되었는지 검증
    private void ValidateUIElements()
    {
        bool hasErrors = false;
        
        if (eventPanel == null)
        {
            Debug.LogError("[TrainEventUIManager] eventPanel이 할당되지 않았습니다!");
            hasErrors = true;
        }
        
        if (eventImage == null)
        {
            Debug.LogError("[TrainEventUIManager] eventImage가 할당되지 않았습니다!");
            hasErrors = true;
        }
        
        if (eventDescription == null)
        {
            Debug.LogError("[TrainEventUIManager] eventDescription이 할당되지 않았습니다!");
            hasErrors = true;
        }
        
        if (choiceButtons == null || choiceButtons.Length < 3)
        {
            Debug.LogError("[TrainEventUIManager] choiceButtons 배열이 null이거나 크기가 3보다 작습니다!");
            hasErrors = true;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (choiceButtons[i] == null)
                {
                    Debug.LogError($"[TrainEventUIManager] choiceButtons[{i}]이(가) null입니다!");
                    hasErrors = true;
                }
            }
        }
        
        if (choiceTexts == null || choiceTexts.Length < 3)
        {
            Debug.LogError("[TrainEventUIManager] choiceTexts 배열이 null이거나 크기가 3보다 작습니다!");
            hasErrors = true;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (choiceTexts[i] == null)
                {
                    Debug.LogError($"[TrainEventUIManager] choiceTexts[{i}]이(가) null입니다!");
                    hasErrors = true;
                }
            }
        }
        
        if (eventChoiceRequirements == null || eventChoiceRequirements.Length < 3)
        {
            Debug.LogError("[TrainEventUIManager] eventChoiceRequirements 배열이 null이거나 크기가 3보다 작습니다!");
            hasErrors = true;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (eventChoiceRequirements[i] == null)
                {
                    Debug.LogError($"[TrainEventUIManager] eventChoiceRequirements[{i}]이(가) null입니다!");
                    hasErrors = true;
                }
                else
                {
                    // 자식 오브젝트 구조 검증
                    Transform imageChild = eventChoiceRequirements[i].transform.Find("RequirementImage");
                    Transform quantityChild = eventChoiceRequirements[i].transform.Find("RequirementQuantity");
                    
                    if (imageChild == null)
                    {
                        Debug.LogError($"[TrainEventUIManager] eventChoiceRequirements[{i}] ('{eventChoiceRequirements[i].name}')에 'RequirementImage' 자식이 없습니다!");
                        hasErrors = true;
                    }
                    else if (imageChild.GetComponent<Image>() == null)
                    {
                        Debug.LogError($"[TrainEventUIManager] eventChoiceRequirements[{i}]의 'RequirementImage'에 Image 컴포넌트가 없습니다!");
                        hasErrors = true;
                    }
                    
                    if (quantityChild == null)
                    {
                        Debug.LogError($"[TrainEventUIManager] eventChoiceRequirements[{i}] ('{eventChoiceRequirements[i].name}')에 'RequirementQuantity' 자식이 없습니다!");
                        hasErrors = true;
                    }
                    else if (quantityChild.GetComponent<TextMeshProUGUI>() == null)
                    {
                        Debug.LogError($"[TrainEventUIManager] eventChoiceRequirements[{i}]의 'RequirementQuantity'에 TextMeshProUGUI 컴포넌트가 없습니다!");
                        hasErrors = true;
                    }
                }
            }
        }
        
        if (fuelItemImage == null)
        {
            Debug.LogWarning("[TrainEventUIManager] fuelItemImage가 할당되지 않았습니다!");
        }
        
        if (foodItemImage == null)
        {
            Debug.LogWarning("[TrainEventUIManager] foodItemImage가 할당되지 않았습니다!");
        }
        
        if (machinePartItemImage == null)
        {
            Debug.LogWarning("[TrainEventUIManager] machinePartItemImage가 할당되지 않았습니다!");
        }
        
        if (passengerItemImage == null)
        {
            Debug.LogWarning("[TrainEventUIManager] passengerItemImage가 할당되지 않았습니다!");
        }
        
        if (hasErrors)
        {
            Debug.LogError("[TrainEventUIManager] === Unity Inspector에서 위의 UI 요소들을 할당해주세요! ===");
        }
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
    
    public string ShowEvent(string eventID)
    {
        if(currentEventID == ""){
            currentEventID = eventID;
        }

        Debug.Log($"[TrainEventUIManager] ShowEvent 호출됨: eventID = {eventID}");
        
        // UI 요소 상태 확인
        Debug.Log($"[TrainEventUIManager] eventPanel null 체크: {eventPanel == null}");
        if (eventPanel != null)
        {
            Debug.Log($"[TrainEventUIManager] eventPanel 활성화 상태: {eventPanel.activeInHierarchy}, 이름: {eventPanel.name}");
        }
        
        if (trainEventManager == null)
        {
            Debug.LogError("[TrainEventUIManager] TrainEventManager가 null입니다!");
            return currentEventID;
        }
        
        TrainEvent trainEvent = trainEventManager.GetTrainEvent(currentEventID);
        if (trainEvent == null)
        {
            Debug.LogWarning($"[TrainEventUIManager] 이벤트 ID '{currentEventID}'를 찾을 수 없습니다!");
            return currentEventID;
        }
        
        Debug.Log($"[TrainEventUIManager] 이벤트 찾음: {trainEvent.GetEventName()}");
        
        if (eventPanel != null)
        {
            Debug.Log("[TrainEventUIManager] eventPanel 존재, 애니메이션 시작");
            
            // 기존 애니메이션만 중지 (개별 코루틴)
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                isTypingAnimation = false;
            }
            
            // 페이드 인 애니메이션과 함께 패널 활성화
            Debug.Log($"[TrainEventUIManager] 코루틴 시작 전 - enabled: {enabled}, gameObject.activeInHierarchy: {gameObject.activeInHierarchy}");
            fadeCoroutine = StartCoroutine(ShowEventWithAnimation(trainEvent));
            Debug.Log($"[TrainEventUIManager] ShowEventWithAnimation 코루틴 시작됨 - fadeCoroutine null 체크: {fadeCoroutine == null}");
        }
        else
        {
            Debug.LogWarning("eventPanel이 할당되지 않았습니다!");
        }
        return currentEventID;
    }
    
    private IEnumerator ShowEventWithAnimation(TrainEvent trainEvent)
    {
        Debug.Log($"[TrainEventUIManager] ===== ShowEventWithAnimation 코루틴 내부 진입 성공! ===== trainEvent null: {trainEvent == null}");
        
        CanvasGroup canvasGroup = eventPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("[TrainEventUIManager] eventPanel에 CanvasGroup 컴포넌트가 없습니다!");
        }
        else
        {
            Debug.Log("[TrainEventUIManager] CanvasGroup 찾음");
        }
        
        Debug.Log("[TrainEventUIManager] SetupEventUI 호출");
        SetupEventUI(trainEvent);
        
        Debug.Log($"[TrainEventUIManager] eventPanel.SetActive(true) 호출 전 - 부모 활성화 상태: {eventPanel.transform.parent?.gameObject.activeInHierarchy}");
        eventPanel.SetActive(true);
        Debug.Log($"[TrainEventUIManager] eventPanel.SetActive(true) 호출 후 - activeInHierarchy: {eventPanel.activeInHierarchy}");
        
        if (canvasGroup != null)
        {
            Debug.Log("[TrainEventUIManager] FadeIn 애니메이션 시작");
            yield return StartCoroutine(FadeIn(canvasGroup));
            Debug.Log("[TrainEventUIManager] FadeIn 애니메이션 완료");
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
    public void OnChoiceClick(string nextEventID, int choiceNumber, Dictionary<ResourceType, int> requirements)
    {
        Debug.Log($"[TrainEventUIManager] OnChoiceClick 호출됨 - nextEventID: {nextEventID}, choiceNumber: {choiceNumber}");
        
        // 타이핑 애니메이션이 활성화되고 진행 중일 때는 클릭 무시
        if (enableTypingAnimation && isTypingAnimation)
        {
            Debug.Log("[TrainEventUIManager] 타이핑 애니메이션 진행 중 - 클릭 무시");
            return;
        }
        
        currentEventID = nextEventID;
        
        // 요구사항이 있으면 리소스 소비
        if (requirements != null && requirements.Count > 0)
        {
            Debug.Log($"[TrainEventUIManager] 요구사항 확인: {requirements.Count}개");
            if (!ConsumeRequirements(requirements))
            {
                Debug.LogError("[TrainEventUIManager] 리소스 소비 실패! 이 버튼은 비활성화되어야 합니다.");
                return;
            }
        }
        
        if (string.IsNullOrEmpty(nextEventID) || nextEventID == "null")
        {
            Debug.Log("[TrainEventUIManager] 다음 이벤트 없음 - 패널 닫기");
            CloseEvent();
            return;
        }
        
        if(trainEventManager.HasEvent(nextEventID)){
            Debug.Log($"[TrainEventUIManager] 다음 이벤트로 이동: {nextEventID}");
            currentEventID = ShowEvent(nextEventID);
        }
        else
        {
            Debug.LogWarning($"[TrainEventUIManager] 이벤트 ID '{nextEventID}'를 찾을 수 없습니다!");
            CloseEvent();
            return;
        }
    }
    
    // 요구사항 리소스를 실제로 소비
    private bool ConsumeRequirements(Dictionary<ResourceType, int> requirements)
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager.Instance가 null입니다!");
            return false;
        }
        
        // 모든 리소스를 소비
        foreach (var requirement in requirements)
        {
            if (!ResourceManager.Instance.ConsumeResources(requirement.Key, requirement.Value))
            {
                Debug.LogError($"{GetResourceDisplayName(requirement.Key)} 소비 실패!");
                return false;
            }
        }
        
        return true;
    }
    
    // UI 요소들을 안전하게 설정하는 메서드
    private void SetupEventUI(TrainEvent trainEvent)
    {
        try
        {
            // 이벤트 이미지 설정
            SetupEventImage(trainEvent);
            
            // 이벤트 설명 설정
            if (eventDescription != null)
            {
                string description = trainEvent.GetEventDescription();
                Debug.Log($"[TrainEventUIManager] 이벤트 설명: {description}");
                
                // 타이핑 애니메이션 활성화 확인
                if (enableTypingAnimation)
                {
                    // 타이핑 애니메이션 시작
                    eventDescription.text = "";  // 먼저 비우기
                    Debug.Log("[TrainEventUIManager] 타이핑 애니메이션 코루틴 시작");
                    typingCoroutine = StartCoroutine(TypeText(eventDescription, description));
                }
                else
                {
                    // 타이핑 애니메이션이 비활성화되어 있으면 즉시 전체 텍스트 표시
                    Debug.Log("[TrainEventUIManager] 타이핑 애니메이션 비활성화 - 전체 텍스트 즉시 표시");
                    eventDescription.text = description;
                }
            }
            else
            {
                Debug.LogError("[TrainEventUIManager] eventDescription이 null입니다!");
            }
            
            // 선택지들 설정
            string[] choiceNextEventIDs = { trainEvent.GetEventChoice1(), trainEvent.GetEventChoice2(), trainEvent.GetEventChoice3() };
            string[] choiceDescriptions = { trainEvent.GetEventChoice1Description(), trainEvent.GetEventChoice2Description(), trainEvent.GetEventChoice3Description() };
            Dictionary<ResourceType, int>[] choiceRequirements = { 
                trainEvent.GetEventChoice1Requirement(), 
                trainEvent.GetEventChoice2Requirement(), 
                trainEvent.GetEventChoice3Requirement() 
            };
            
            // 배열 null 체크
            if (choiceButtons == null || choiceButtons.Length < 3)
            {
                Debug.LogError("choiceButtons 배열이 null이거나 크기가 3보다 작습니다!");
                return;
            }
            if (choiceTexts == null || choiceTexts.Length < 3)
            {
                Debug.LogError("choiceTexts 배열이 null이거나 크기가 3보다 작습니다!");
                return;
            }
            if (eventChoiceRequirements == null || eventChoiceRequirements.Length < 3)
            {
                Debug.LogError("eventChoiceRequirements 배열이 null이거나 크기가 3보다 작습니다!");
                return;
            }
            
            for (int i = 0; i < 3; i++)
            {
                // 각 요소 null 체크
                if (choiceButtons[i] == null)
                {
                    Debug.LogError($"choiceButtons[{i}]이(가) null입니다! Unity Inspector에서 할당해주세요.");
                    continue;
                }
                if (choiceTexts[i] == null)
                {
                    Debug.LogError($"choiceTexts[{i}]이(가) null입니다! Unity Inspector에서 할당해주세요.");
                    continue;
                }
                if (eventChoiceRequirements[i] == null)
                {
                    Debug.LogError($"eventChoiceRequirements[{i}]이(가) null입니다! Unity Inspector에서 할당해주세요.");
                    continue;
                }
                
                SetupChoice(choiceButtons[i], choiceTexts[i], 
                eventChoiceRequirements[i], choiceNextEventIDs[i], choiceDescriptions[i], choiceRequirements[i], i + 1);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UI 요소 설정 중 오류 발생: {e.Message}");
            Debug.LogError($"스택 트레이스: {e.StackTrace}");
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
    
    // 선택지 설정 (요구사항 GameObject 포함)
    private void SetupChoice(Button button, TextMeshProUGUI text, GameObject eventChoiceRequirement, 
                            string nextEventID, string choiceDescription, Dictionary<ResourceType, int> requirements, int choiceNumber)
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
            // 선택지 설명 표시
            text.text = choiceDescription;
            
            // 요구사항 표시
            SetupRequirementUI(eventChoiceRequirement, requirements);
            
            // 요구사항을 충족하는지 확인
            bool canAfford = CanAffordRequirements(requirements);
            
            // 버튼 활성화/비활성화 및 시각적 표시
            SetButtonInteractable(button, text, canAfford);
            SetButtonActive(button, true);
            
            // 기존 리스너 제거 후 새 리스너 추가
            button.onClick.RemoveAllListeners();
            if (canAfford)
            {
                Debug.Log($"[TrainEventUIManager] 선택지 {choiceNumber} 버튼 리스너 등록 (활성화) - nextEventID: {nextEventID}");
                button.onClick.AddListener(() => OnChoiceClick(nextEventID, choiceNumber, requirements));
            }
            else
            {
                Debug.Log($"[TrainEventUIManager] 선택지 {choiceNumber} 버튼 리스너 등록 (비활성화)");
                // 요구사항을 충족하지 못하면 클릭해도 아무 동작 안함
                button.onClick.AddListener(() => OnInsufficientResourcesClick(requirements));
            }

        }
        else
        {
            Debug.LogWarning($"선택지 {choiceNumber}가 없습니다.");
            SetButtonActive(button, false);
            
            // 요구사항 오브젝트도 비활성화
            if (eventChoiceRequirement != null)
            {
                eventChoiceRequirement.SetActive(false);
            }
        }
    }
    
    // 요구사항을 충족할 수 있는지 확인
    private bool CanAffordRequirements(Dictionary<ResourceType, int> requirements)
    {
        // 요구사항이 없으면 항상 가능
        if (requirements == null || requirements.Count == 0)
            return true;
        
        // ResourceManager가 없으면 기본적으로 허용
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("ResourceManager.Instance가 null입니다. 요구사항 체크를 건너뜁니다.");
            return true;
        }
        
        // 모든 요구사항을 확인
        foreach (var requirement in requirements)
        {
            int currentAmount = ResourceManager.Instance.GetResource(requirement.Key);
            if (currentAmount < requirement.Value)
            {
                return false; // 하나라도 부족하면 false
            }
        }
        
        return true; // 모든 요구사항 충족
    }
    
    // 버튼의 상호작용 가능 여부 설정 및 시각적 표시
    private void SetButtonInteractable(Button button, TextMeshProUGUI text, bool interactable)
    {
        button.interactable = interactable;
        
        // 시각적 피드백: 비활성화 시 텍스트 색상 변경
        if (text != null)
        {
            text.color = interactable ? Color.black : Color.red; // 요구사항 미충족 시 빨간색
        }
    }
    
    // 자원이 부족할 때 클릭 처리
    private void OnInsufficientResourcesClick(Dictionary<ResourceType, int> requirements)
    {
        Debug.Log("[TrainEventUIManager] OnInsufficientResourcesClick 호출됨");
        
        if (requirements == null || requirements.Count == 0)
            return;
        
        // 부족한 자원 목록 만들기
        string insufficientResources = "";
        foreach (var requirement in requirements)
        {
            int currentAmount = ResourceManager.Instance.GetResource(requirement.Key);
            if (currentAmount < requirement.Value)
            {
                string resourceName = GetResourceDisplayName(requirement.Key);
                insufficientResources += $"{resourceName} (필요: {requirement.Value}, 보유: {currentAmount})\n";
            }
        }
        
        Debug.LogWarning($"[TrainEventUIManager] 자원이 부족합니다:\n{insufficientResources}");
        // 여기에 UI로 알림 표시를 추가할 수 있습니다
    }

    private void SetupRequirementUI(GameObject eventChoiceRequirement, Dictionary<ResourceType, int> requirement)
    {
        if (eventChoiceRequirement == null)
        {
            Debug.LogWarning("requirementObject가 할당되지 않았습니다.");
            return;
        }
        
        if (requirement == null || requirement.Count == 0)
        {
            eventChoiceRequirement.SetActive(false);
            return;
        }
        
        // 요구사항이 하나만 있을 경우 (가장 흔한 경우)
        if (requirement.Count == 1)
        {
            // 자식 오브젝트 찾기 (안전하게)
            Transform imageTransform = eventChoiceRequirement.transform.Find("RequirementImage");
            Transform quantityTransform = eventChoiceRequirement.transform.Find("RequirementQuantity");
            
            if (imageTransform == null)
            {
                Debug.LogError($"'{eventChoiceRequirement.name}' 오브젝트에서 'RequirementImage' 자식 오브젝트를 찾을 수 없습니다!");
                eventChoiceRequirement.SetActive(false);
                return;
            }
            
            if (quantityTransform == null)
            {
                Debug.LogError($"'{eventChoiceRequirement.name}' 오브젝트에서 'RequirementQuantity' 자식 오브젝트를 찾을 수 없습니다!");
                eventChoiceRequirement.SetActive(false);
                return;
            }
            
            Image requirementObjectImage = imageTransform.GetComponent<Image>();
            TextMeshProUGUI requirementObjectQuantity = quantityTransform.GetComponent<TextMeshProUGUI>();
            
            if (requirementObjectImage == null)
            {
                Debug.LogError($"'RequirementImage' 오브젝트에 Image 컴포넌트가 없습니다!");
                eventChoiceRequirement.SetActive(false);
                return;
            }
            
            if (requirementObjectQuantity == null)
            {
                Debug.LogError($"'RequirementQuantity' 오브젝트에 TextMeshProUGUI 컴포넌트가 없습니다!");
                eventChoiceRequirement.SetActive(false);
                return;
            }
            
            foreach (var req in requirement)
            {
                if (requirementObjectImages.ContainsKey(req.Key))
                {
                    requirementObjectImage.sprite = requirementObjectImages[req.Key];
                    requirementObjectQuantity.text = req.Value.ToString();
                    
                    // 리소스가 부족하면 빨간색으로 표시
                    bool hasEnoughResources = true;
                    if (ResourceManager.Instance != null)
                    {
                        int currentAmount = ResourceManager.Instance.GetResource(req.Key);
                        hasEnoughResources = currentAmount >= req.Value;
                        
                        // 부족하면 빨간색, 충분하면 흰색
                        Color displayColor = hasEnoughResources ? Color.white : Color.red;
                        requirementObjectImage.color = displayColor;
                        requirementObjectQuantity.color = displayColor;
                    }
                    
                    eventChoiceRequirement.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"리소스 타입 {req.Key}에 대한 이미지가 없습니다.");
                    eventChoiceRequirement.SetActive(false);
                }
            }
        }
    }
    
    // 리소스 타입을 한글 표시명으로 변환
    private string GetResourceDisplayName(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                return "식량";
            case ResourceType.Fuel:
                return "연료";
            case ResourceType.Passenger:
                return "승객";
            case ResourceType.MachinePart:
                return "부품";
            case ResourceType.Durability:
                return "내구도";
            default:
                return resourceType.ToString();
        }
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
        Dictionary<ResourceType, int>[] choiceRequirements = { 
            trainEvent.GetEventChoice1Requirement(), 
            trainEvent.GetEventChoice2Requirement(), 
            trainEvent.GetEventChoice3Requirement() 
        };
        
        for (int i = 0; i < 3; i++)
        {
            GameObject requirementObject = (eventChoiceRequirements != null && i < eventChoiceRequirements.Length) ? eventChoiceRequirements[i] : null;
            SetupChoice(choiceButtons[i], choiceTexts[i], requirementObject, choiceNextEventIDs[i], choiceDescriptions[i], choiceRequirements[i], i + 1);
        }
        

    }

}
