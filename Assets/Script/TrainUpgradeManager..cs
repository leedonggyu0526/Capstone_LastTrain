using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TrainUpgradeManager : MonoBehaviour
{
    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    [Header("Upgrade Settings")]
    public int level = 1;
    public int baseFoodCost = 50;
    public int basePartCost = 50;

    [Header("Linked Resource Producers")]
    public List<ResourceProducer> resourceProducers; // Inspector에서 연결

    private void Start()
    {
        UpdateTooltipText();
        HideTooltip();
        UpdateProductionMultiplier(); // 시작 시 초기 multiplier 적용
    }

    public void ShowTooltip()
    {
        tooltipPanel?.SetActive(true);
        UpdateTooltipText();
    }

    public void HideTooltip()
    {
        tooltipPanel?.SetActive(false);
    }

    public void Upgrade()
    {
        int foodCost = baseFoodCost * (int)Mathf.Pow(2, level - 1);
        int partCost = basePartCost * (int)Mathf.Pow(2, level - 1);

        bool hasFood = ResourceManager.Instance.GetResource(ResourceType.Food) >= foodCost;
        bool hasPart = ResourceManager.Instance.GetResource(ResourceType.MachinePart) >= partCost;

        if (hasFood && hasPart)
        {
            ResourceManager.Instance.AddResource(ResourceType.Food, -foodCost);
            ResourceManager.Instance.AddResource(ResourceType.MachinePart, -partCost);

            level++;
            UpdateTooltipText();
            UpdateProductionMultiplier(); // ⬅️ 업그레이드마다 multiplier 적용
            Debug.Log("Upgrade successful!");
        }
        else
        {
            Debug.Log("Not enough resources.");
        }
    }

    private void UpdateTooltipText()
    {
        if (tooltipText == null) return;

        int foodCost = baseFoodCost * (int)Mathf.Pow(2, level - 1);
        int partCost = basePartCost * (int)Mathf.Pow(2, level - 1);

        tooltipText.text = $"Level: {level}\nCost:\nFood: {foodCost}, Part: {partCost}";
    }

    private void UpdateProductionMultiplier()
    {
        float multiplier = 1f + (level - 1) * 0.2f;
        foreach (var producer in resourceProducers)
        {
            if (producer != null)
                producer.SetMultiplier(multiplier);
        }
    }
}
