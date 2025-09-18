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
		// ResourceManager가 아직 초기화되지 않았다면 대기
		if (ResourceManager.Instance == null)
		{
			Debug.LogWarning("[ResourceUIManager] ResourceManager.Instance가 없습니다. 씬에 ResourceManager가 있는지 확인하세요.");
			return;
		}

		foreach (var rui in resourceUIs)
		{
			// UI 참조가 비어 있으면 건너뛰고 경고
			if (rui.countText == null)
			{
				Debug.LogWarning($"[ResourceUIManager] {rui.type} 의 countText가 비어 있습니다. Inspector에서 할당하세요.");
				continue;
			}

			int current = ResourceManager.Instance.GetResource(rui.type);
			int max = ResourceManager.Instance.GetMaxCapacity(rui.type);
			rui.countText.text = $"{current}/{max}";
			// 참조가 비어 있으면 건너뛰고 경고
		}
    }
}
