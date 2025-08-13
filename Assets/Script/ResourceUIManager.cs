using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public struct ResourceUI
{
    public ResourceType type;   // Fuel, Food, MachinePart, Passenger
    public Image icon;          // Inspector에서 연결해야하는 아이콘
    public TMP_Text countText;      // 예:500/1000과 같이 표시할 Text
}

public class ResourceUIManager : MonoBehaviour
{
    [Header("자원별 UI 컴포넌트 텍스트")]
    public List<ResourceUI> resourceUIs = new List<ResourceUI>();

    void Start()
    {
        // 초기 한번 업데이트
        UpdateAllUI();
    }

    void Update()
    {
        // 실시간 업데이트가 필요한 경우 여기서만 업데이트해도 되고,
        // 필요에 따라서는 이벤트 방식(UpdateAllUI() 호출)으로도 가능합니다.
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        foreach (var rui in resourceUIs)
        {
            int current = ResourceManager.Instance.GetResource(rui.type);
            int max = ResourceManager.Instance.GetMaxCapacity(rui.type);
            rui.countText.text = $"{current}/{max}";
            // 아이콘은 이미 Inspector에서 할당된 상태라면 별도 업데이트 불필요
        }
    }
}
