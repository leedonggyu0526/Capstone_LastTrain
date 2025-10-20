using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RepairUIManager : UIManager
{
    [Header("수리 UI 요소")]
    public GameObject repairPanel;
    public TextMeshProUGUI repairResultText;
    public Button repairButton;
    
    [Header("수리 비용")]
    public int fuelCost = 150;
    public int machinePartCost = 100;
    public int foodCost = 100;
    public int passengerCost = 100;
    
    // 수리 상태 관리
    private bool isRepairCompleted = false;
    
    // 자동으로 찾은 CanvasGroup
    private CanvasGroup repairCanvasGroup;
    
    protected override void Start()
    {
        base.Start(); // UIManager의 Start 호출
        
        // CanvasGroup 자동 찾기
        FindCanvasGroup();
        
        // 수리 버튼 이벤트 리스너 설정
        if (repairButton != null)
        {
            repairButton.onClick.RemoveAllListeners();
            repairButton.onClick.AddListener(OnRepairButtonClick);
        }
    }
    
    /// <summary>
    /// CanvasGroup 자동 찾기
    /// </summary>
    private void FindCanvasGroup()
    {
        if (repairPanel != null)
        {
            // repairPanel에서 CanvasGroup 찾기
            repairCanvasGroup = repairPanel.GetComponent<CanvasGroup>();
            
            // 없으면 자동으로 추가
            if (repairCanvasGroup == null)
            {
                repairCanvasGroup = repairPanel.AddComponent<CanvasGroup>();
            }
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy(); // UIManager의 OnDestroy 호출
        
        // 수리 관련 정리
        StopAllAnimations();
    }
    
    protected override void OnCloseButtonClick()
    {
        // 수리 패널 닫기
        HideRepairPanel();
    }
    
    /// <summary>
    /// 수리 패널 표시
    /// </summary>
    public void ShowRepairPanel()
    {
        if (repairPanel != null)
        {
            repairPanel.SetActive(true);
            isRepairCompleted = false;
            
            // 결과 텍스트 초기화
            if (repairResultText != null)
            {
                repairResultText.text = "수리를 진행하시겠습니까?\n\n" +
                    $"필요한 리소스:\n" +
                    $"연료: {fuelCost}\n" +
                    $"기계부품: {machinePartCost}\n" +
                    $"식량: {foodCost}\n" +
                    $"승객: {passengerCost}";
                repairResultText.color = Color.black;
            }
            
            // 버튼 텍스트 설정
            if (repairButton != null)
            {
                TextMeshProUGUI buttonText = repairButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = "수리하기";
                }
            }
            
            // 페이드인 애니메이션
            if (repairCanvasGroup != null)
            {
                repairCanvasGroup.alpha = 0f; // 알파값 초기화
                StopAllAnimations();
                fadeCoroutine = StartCoroutine(FadeIn(repairCanvasGroup));
            }
        }
    }
    
    /// <summary>
    /// 수리 패널 숨기기
    /// </summary>
    public void HideRepairPanel()
    {
        if (repairPanel != null)
        {
            // 페이드아웃 애니메이션
            if (repairCanvasGroup != null)
            {
                StopAllAnimations();
                fadeCoroutine = StartCoroutine(FadeOut(repairCanvasGroup));
                StartCoroutine(DeactivateAfterFade());
            }
            else
            {
                repairPanel.SetActive(false);
                isRepairCompleted = false;
            }
        }
    }
    
    /// <summary>
    /// 페이드아웃 후 패널 비활성화
    /// </summary>
    private System.Collections.IEnumerator DeactivateAfterFade()
    {
        yield return new WaitForSeconds(1f / fadeSpeed);
        
        if (repairPanel != null)
        {
            repairPanel.SetActive(false);
            isRepairCompleted = false;
        }
    }
    
    /// <summary>
    /// 통합된 수리 버튼 클릭 처리
    /// </summary>
    private void OnRepairButtonClick()
    {
        if (!isRepairCompleted)
        {
            // 수리 실행
            ExecuteRepair();
        }
    }
    
    /// <summary>
    /// 수리 실행
    /// </summary>
    private void ExecuteRepair()
    {
        // 수리 전 리소스 상태 확인
        if (ResourceManager.Instance != null)
        {
            int fuelBefore = ResourceManager.Instance.GetResource(ResourceType.Fuel);
            int machinePartBefore = ResourceManager.Instance.GetResource(ResourceType.MachinePart);
            int foodBefore = ResourceManager.Instance.GetResource(ResourceType.Food);
            int passengerBefore = ResourceManager.Instance.GetResource(ResourceType.Passenger);
            
            // 리소스 소비
            bool fuelConsumed = ResourceManager.Instance.ConsumeResources(ResourceType.Fuel, fuelCost);
            bool machinePartConsumed = ResourceManager.Instance.ConsumeResources(ResourceType.MachinePart, machinePartCost);
            bool foodConsumed = ResourceManager.Instance.ConsumeResources(ResourceType.Food, foodCost);
            bool passengerConsumed = ResourceManager.Instance.ConsumeResources(ResourceType.Passenger, passengerCost);
            
            // 수리 성공/실패 판정
            bool repairSuccess = fuelConsumed && machinePartConsumed && foodConsumed && passengerConsumed;
            
            // 수리 결과 표시
            ShowRepairResult(repairSuccess, fuelBefore, machinePartBefore, foodBefore, passengerBefore);
        }
        else
        {
            ShowRepairResult(false, 0, 0, 0, 0);
        }
    }
    
    /// <summary>
    /// 수리 결과를 패널에 표시
    /// </summary>
    private void ShowRepairResult(bool success, int fuelBefore, int machinePartBefore, int foodBefore, int passengerBefore)
    {
        isRepairCompleted = true;
        
        if (repairResultText != null)
        {
            if (success)
            {
                repairResultText.text = "수리 완료!\n\n" +
                    $"연료: {fuelBefore} → {ResourceManager.Instance.GetResource(ResourceType.Fuel)}\n" +
                    $"기계부품: {machinePartBefore} → {ResourceManager.Instance.GetResource(ResourceType.MachinePart)}\n" +
                    $"식량: {foodBefore} → {ResourceManager.Instance.GetResource(ResourceType.Food)}\n" +
                    $"승객: {passengerBefore} → {ResourceManager.Instance.GetResource(ResourceType.Passenger)}";
                repairResultText.color = Color.green;
            }
            else
            {
                repairResultText.text = "수리 실패!\n\n" +
                    "필요한 리소스가 부족합니다.\n" +
                    $"연료: {ResourceManager.Instance.GetResource(ResourceType.Fuel)}/{fuelCost}\n" +
                    $"기계부품: {ResourceManager.Instance.GetResource(ResourceType.MachinePart)}/{machinePartCost}\n" +
                    $"식량: {ResourceManager.Instance.GetResource(ResourceType.Food)}/{foodCost}\n" +
                    $"승객: {ResourceManager.Instance.GetResource(ResourceType.Passenger)}/{passengerCost}";
                repairResultText.color = Color.red;
            }
        }
        
        // 버튼 텍스트 변경
        if (repairButton != null)
        {
            TextMeshProUGUI buttonText = repairButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "확인";
            }
        }
    }
    
    /// <summary>
    /// 수리 비용 설정
    /// </summary>
    public void SetRepairCosts(int fuel, int machinePart, int food, int passenger)
    {
        fuelCost = fuel;
        machinePartCost = machinePart;
        foodCost = food;
        passengerCost = passenger;
    }
    
    /// <summary>
    /// 수리 패널 토글 (표시/숨김)
    /// </summary>
    public void ToggleRepairPanel()
    {
        if (repairPanel != null && repairPanel.activeSelf)
        {
            HideRepairPanel();
        }
        else
        {
            ShowRepairPanel();
        }
    }
}
