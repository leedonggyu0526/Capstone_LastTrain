using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainUpgradeManager : MonoBehaviour
{
    [Header("���׷��̵� ����")]
    public int level = 1;                        // ���� ���׷��̵� ����
    public float baseFoodCost = 50f;             // ù ���׷��̵��� �ķ� ���
    public float basePartCost = 50f;             // ù ���׷��̵��� �ӽ� ��Ʈ ���
    public float costMultiplier = 2f;            // ���׷��̵��Ҽ��� ��� ���

    [Header("���� �ý���")]
    public ResourceProducer foodProducer;        // �ķ� ���� ������Ʈ ����
    public ResourceProducer partProducer;        // �ӽ� ��Ʈ ���� ������Ʈ ����
    public float productionMultiplier = 1.2f;    // ���׷��̵� �� ���귮 20% ����

    [Header("UI ����")]
    public Button upgradeButton;                 // ���׷��̵� ��ư
    public GameObject tooltipPanel;              // ���� �г� (�⺻ ��Ȱ��ȭ)
    public TMP_Text tooltipText;                 // ���� ���� �ؽ�Ʈ

    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;

        // ���� �⺻ �����
        tooltipPanel.SetActive(false);

        // ��ư Ŭ�� �̺�Ʈ ���
        upgradeButton.onClick.AddListener(Upgrade);

        // �ʱ� ���� �ؽ�Ʈ ǥ��
        UpdateTooltipText();
    }

    // ���� �ؽ�Ʈ ���� (���� �� ���� ��� ǥ��)
    private void UpdateTooltipText()
    {
        float foodCost = baseFoodCost * Mathf.Pow(costMultiplier, level - 1);
        float partCost = basePartCost * Mathf.Pow(costMultiplier, level - 1);
        tooltipText.text = $"Lv.{level}\nFood: {foodCost}\nPart: {partCost}";
    }

    // ���׷��̵� ���� (��ư Ŭ�� �� ȣ���)
    private void Upgrade()
    {
        float foodCost = baseFoodCost * Mathf.Pow(costMultiplier, level - 1);
        float partCost = basePartCost * Mathf.Pow(costMultiplier, level - 1);

        if (resourceManager.GetResource(ResourceType.Food) >= foodCost &&
            resourceManager.GetResource(ResourceType.MachinePart) >= partCost)
        {
            // �ڿ� �Һ�
            resourceManager.AddResource(ResourceType.Food, -(int)foodCost);
            resourceManager.AddResource(ResourceType.MachinePart, -(int)partCost);

            level++;  // ���� ����
            IncreaseProduction(); // ���귮 ���� ó��
            UpdateTooltipText(); // ���� ���� ����
        }
        else
        {
            Debug.Log("�ڿ��� �����մϴ�!");
        }
    }

    // ���귮 ���� ó��
    private void IncreaseProduction()
    {
        // �ķ� ���� ������Ʈ�� ����Ǿ� �ְ�, Ÿ���� Food�� ��
        if (foodProducer != null && foodProducer.resourceType == ResourceType.Food)
        {
            foodProducer.amountPerCycle = Mathf.CeilToInt(foodProducer.amountPerCycle * productionMultiplier);
            Debug.Log($"[Food] ���귮 ����: {foodProducer.amountPerCycle}");
        }

        // �ӽ� ��Ʈ ���� ������Ʈ�� ����Ǿ� �ְ�, Ÿ���� MachinePart�� ��
        if (partProducer != null && partProducer.resourceType == ResourceType.MachinePart)
        {
            partProducer.amountPerCycle = Mathf.CeilToInt(partProducer.amountPerCycle * productionMultiplier);
            Debug.Log($"[Part] ���귮 ����: {partProducer.amountPerCycle}");
        }
    }

    // ���콺�� ��ư�� �÷��� �� ���� ǥ��
    public void ShowTooltip()
    {
        tooltipPanel.SetActive(true);
    }

    // ���콺�� ��ư���� ���� �� ���� ����
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
