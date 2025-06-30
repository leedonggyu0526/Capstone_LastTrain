using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public Transform gridParent;
    public GameObject itemSlotPrefab;
    public GameObject inventoryPanel;

    public Image detailImage;
    public TextMeshProUGUI detailName;
    public TextMeshProUGUI detailEffect;
    public TextMeshProUGUI detailDescription;
    // 가지고 있는 템 리스트
    public List<Item> ownedItems;

    void Start()
    {
        PopulateInventory();
    }
    /// <summary>
    /// gridParent 안에 있는 이전에 만들어진 슬롯들을 전부 삭제 및 ownedItems 순회 프리팹 생성
    /// </summary>
    void PopulateInventory()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (var item in ownedItems)
        {
            GameObject slot = Instantiate(itemSlotPrefab, gridParent);
            slot.GetComponent<Image>().sprite = item.artwork;

            Button btn = slot.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowItemDetail(item));
        }
    }

    void ShowItemDetail(Item item)
    {
        detailImage.sprite = item.artwork;
        detailName.text = item.itemName;

        // 예시 효과 (아이템 이름에 따라 조건적으로 설정해도 됨)
        if (item.itemName.Contains("빵"))
            detailEffect.text = "<color=green>식량 생산량 +35%</color>";
        else
            detailEffect.text = item.itemEffect;

        detailDescription.text = item.description;
    }
    // Open/Close
    public void OpenInventoryPanel()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            PopulateInventory(); // 열 때 새로고침할 경우
        }
    }
    public void CloseInventoryPanel()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    public void AddItem(Item newItem)
    {
        if (!ownedItems.Contains(newItem))
        {
            ownedItems.Add(newItem);
            Debug.Log($"아이템 '{newItem.itemName}'을(를) 획득했습니다.");
        }
        else
        {
            Debug.Log($"이미 '{newItem.itemName}' 아이템을 가지고 있습니다.");
        }

        PopulateInventory(); // UI 새로고침
    }
    /// <summary>
    /// 아이템 소유 판정
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public bool HasItemByID(int itemID)
    {
        return ownedItems.Exists(item => item.ID == itemID);
    }
}
