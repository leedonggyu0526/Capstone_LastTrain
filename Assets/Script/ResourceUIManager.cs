using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct ResourceUI
{
    public ResourceType type;   // Fuel, Food, MachinePart, Passenger
    public Image icon;          // Inspector�� ��Ӵٿ����� �Ҵ�
    public TMP_Text countText;      // ��500/1000�� �� ǥ���� Text
}

public class ResourceUIManager : MonoBehaviour
{
    [Header("�ڿ��� UI �����ܰ� �ؽ�Ʈ")]
    public List<ResourceUI> resourceUIs = new List<ResourceUI>();

    void Start()
    {
        // �ʱ� �ѹ� ����
        UpdateAllUI();
    }

    void Update()
    {
        // �ǽð� ������ ������ �� �����Ӹ��� �����ص� �ǰ�,
        // ������ ���ٸ� �̺�Ʈ ���(UpdateAllUI() ȣ��)���ε� ����մϴ�.
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        foreach (var rui in resourceUIs)
        {
            int current = ResourceManager.Instance.GetResource(rui.type);
            int max = ResourceManager.Instance.GetMaxCapacity(rui.type);
            rui.countText.text = $"{current}/{max}";
            // �����ܴ� �̸� Inspector���� �Ҵ��� �ξ��ٸ� ���� ������Ʈ ���ʿ�
        }
    }
}
