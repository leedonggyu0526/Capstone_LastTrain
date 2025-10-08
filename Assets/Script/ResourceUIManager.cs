using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct ResourceUI
{
    public ResourceType type;   // Fuel, Food, MachinePart, Passenger
    public Image icon;          // Inspector에서 아이콘 이미지 할당
    public TMP_Text countText;  // 예: 500/1000 형태로 표시
}

public class ResourceUIManager : MonoBehaviour
{
    [Header("자원 UI 아이콘 및 텍스트")]
    public List<ResourceUI> resourceUIs = new List<ResourceUI>();

    void Start()
    {
        // 초기 한 번 갱신
        UpdateAllUI();
    }

    void Update()
    {
        // 실제 게임에서는 이벤트 기반(값 변경 시에만 UpdateAllUI 호출)으로 바꾸는 것을 권장합니다.
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
