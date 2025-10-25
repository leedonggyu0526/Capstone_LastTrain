using UnityEngine;
using UnityEngine.UI;

public class ResourceEcho : MonoBehaviour
{
    [Header("버튼 설정")]
    public Button[] echoButtons; // Echo를 실행할 버튼들 (Inspector에서 할당)
    
    private void Start()
    {
        // 모든 버튼에 Echo 메서드를 리스너로 등록
        if (echoButtons != null && echoButtons.Length > 0)
        {
            foreach (Button button in echoButtons)
            {
                if (button != null)
                {
                    button.onClick.AddListener(Echo);
                    Debug.Log($"[ResourceEcho] 버튼 '{button.name}'에 Echo 리스너 등록됨");
                }
            }
        }
        else
        {
            Debug.LogWarning("[ResourceEcho] echoButtons가 할당되지 않았습니다!");
        }
    }
    
    private void OnDestroy()
    {
        // 메모리 누수 방지를 위해 리스너 제거
        if (echoButtons != null)
        {
            foreach (Button button in echoButtons)
            {
                if (button != null)
                {
                    button.onClick.RemoveListener(Echo);
                }
            }
        }
    }
    
    // 전체 리소스 양을 0으로 초기화
    public void Echo()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[ResourceEcho] ResourceManager.Instance가 null입니다!");
            return;
        }
        
        ResourceManager.Instance.SetResource(ResourceType.Fuel, 0);
        ResourceManager.Instance.SetResource(ResourceType.Food, 0);
        ResourceManager.Instance.SetResource(ResourceType.MachinePart, 0);
        ResourceManager.Instance.SetResource(ResourceType.Passenger, 0);
        ResourceManager.Instance.SetResource(ResourceType.Durability, 0);
        
        Debug.Log("[ResourceEcho] 전체 리소스 양을 0으로 초기화");
    }
}
