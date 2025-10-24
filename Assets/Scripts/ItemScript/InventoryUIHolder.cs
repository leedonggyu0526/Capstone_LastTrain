using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// InventoryUIManager가 씬에서 UI 요소를 찾을 수 있도록 정보를 담는 컴포넌트입니다.
/// 인벤토리 UI의 최상위 오브젝트(예: InventoryPanel)에 이 스크립트를 추가하고,
/// 하위 UI 요소들을 인스펙터에서 할당해주세요.
/// </summary>
public class InventoryUIHolder : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public Transform gridParent;

    [Header("Item Details")]
    public Image detailImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailEffect;
    public TextMeshProUGUI detailDescription;
}