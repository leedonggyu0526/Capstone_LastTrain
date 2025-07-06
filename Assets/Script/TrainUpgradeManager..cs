using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainUpgradeManager : MonoBehaviour
{
    [Header("업그레이드 설정")]
    public int level = 1;                        // 현재 업그레이드 레벨
    public float baseFoodCost = 50f;             // 첫 업그레이드의 식량 비용
    public float basePartCost = 50f;             // 첫 업그레이드의 머신 파트 비용
    public float costMultiplier = 2f;            // 업그레이드할수록 비용 배수

    [Header("생산 시스템")]
    public ResourceProducer foodProducer;        // 식량 생산 오브젝트 연결
    public ResourceProducer partProducer;        // 머신 파트 생산 오브젝트 연결
    public float productionMultiplier = 1.2f;    // 업그레이드 시 생산량 20% 증가

    [Header("UI 참조")]
    public Button upgradeButton;                 // 업그레이드 버튼
    public GameObject tooltipPanel;              // 툴팁 패널 (기본 비활성화)
    public TMP_Text tooltipText;                 // 툴팁 안의 텍스트

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;

        // 툴팁 기본 숨기기
        tooltipPanel.SetActive(false);

        // 버튼 클릭 이벤트 등록
        upgradeButton.onClick.AddListener(Upgrade);

        // 초기 툴팁 텍스트 표시
        UpdateTooltipText();
    }

    // 툴팁 텍스트 갱신 (레벨 및 현재 비용 표시)
    private void UpdateTooltipText()
    {
        float foodCost = baseFoodCost * Mathf.Pow(costMultiplier, level - 1);
        float partCost = basePartCost * Mathf.Pow(costMultiplier, level - 1);
        tooltipText.text = $"Lv.{level}\nFood: {foodCost}\nPart: {partCost}";
    }

    // 업그레이드 실행 (버튼 클릭 시 호출됨)
    private void Upgrade()
    {
        float foodCost = baseFoodCost * Mathf.Pow(costMultiplier, level - 1);
        float partCost = basePartCost * Mathf.Pow(costMultiplier, level - 1);

        if (resourceManager.GetResource(ResourceType.Food) >= foodCost &&
            resourceManager.GetResource(ResourceType.MachinePart) >= partCost)
        {
            // 자원 소비
            resourceManager.AddResource(ResourceType.Food, -(int)foodCost);
            resourceManager.AddResource(ResourceType.MachinePart, -(int)partCost);

            level++;  // 레벨 증가
            IncreaseProduction(); // 생산량 증가 처리
            UpdateTooltipText(); // 툴팁 내용 갱신
        }
        else
        {
            Debug.Log("자원이 부족합니다!");
        }
    }

    // 생산량 증가 처리
    private void IncreaseProduction()
    {
        // 식량 생산 오브젝트가 연결되어 있고, 타입이 Food일 때
        if (foodProducer != null && foodProducer.resourceType == ResourceType.Food)
        {
            foodProducer.amountPerCycle = Mathf.CeilToInt(foodProducer.amountPerCycle * productionMultiplier);
            Debug.Log($"[Food] 생산량 증가: {foodProducer.amountPerCycle}");
        }

        // 머신 파트 생산 오브젝트가 연결되어 있고, 타입이 MachinePart일 때
        if (partProducer != null && partProducer.resourceType == ResourceType.MachinePart)
        {
            partProducer.amountPerCycle = Mathf.CeilToInt(partProducer.amountPerCycle * productionMultiplier);
            Debug.Log($"[Part] 생산량 증가: {partProducer.amountPerCycle}");
        }
    }

    // 마우스를 버튼에 올렸을 때 툴팁 표시
    public void ShowTooltip()
    {
        tooltipPanel.SetActive(true);
    }

    // 마우스를 버튼에서 뗐을 때 툴팁 숨김
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
