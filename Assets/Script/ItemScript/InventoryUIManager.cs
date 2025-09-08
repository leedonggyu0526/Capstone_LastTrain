using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    public static InventoryUIManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시 파괴 방지
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }
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
            GetComponent<ItemNoticer>().ShowNotification(newItem.itemName);
        }
        else
        {
            Debug.Log($"이미 '{newItem.itemName}' 아이템을 가지고 있습니다.");
            GetComponent<ItemNoticer>().ShowNotification($"이미 '{newItem.itemName}' 아이템을 가지고 있습니다.");
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
        return ownedItems.Any(item => item.ID == itemID);
    }
}
