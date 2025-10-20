using UnityEngine;
using UnityEngine.UI;

public enum InventoryButtonAction
{
    Open,
    Close
}

/// <summary>
/// 인벤토리 UI를 열거나 닫는 버튼 스크립트입니다.
/// </summary>
public class InventoryButton : MonoBehaviour
{
    [Tooltip("이 버튼이 수행할 동작을 선택")]
    public InventoryButtonAction buttonAction;

    private Button inventoryButton;

    private void Start()
    {
        inventoryButton = GetComponent<Button>();
        inventoryButton.onClick.RemoveAllListeners();

        if (InventoryUIManager.Instance != null)
        {
            // 인스펙터에서 설정한 동작에 따라 적절한 함수를 연결합니다.
            switch (buttonAction)
            {
                case InventoryButtonAction.Open:
                    inventoryButton.onClick.AddListener(InventoryUIManager.Instance.OpenInventoryPanel);
                    Debug.Log("인벤토리 열기 버튼 기능이 연결되었습니다.");
                    break;
                case InventoryButtonAction.Close:
                    inventoryButton.onClick.AddListener(InventoryUIManager.Instance.CloseInventoryPanel);
                    Debug.Log("인벤토리 닫기 버튼 기능이 연결되었습니다.");
                    break;
            }
        }
        else
        {
            Debug.LogError("InventoryUIManager 인스턴스를 찾을 수 없어 버튼 기능을 연결할 수 없습니다.");
            inventoryButton.interactable = false; // 인스턴스가 없으면 버튼 비활성화
        }
    }
}