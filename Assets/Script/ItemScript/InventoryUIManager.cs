using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject itemSlotPrefab;
    
    [Header("Data (Persistent)")]
    public List<Item> ownedItems;

    public static InventoryUIManager Instance { get; private set; }
    private InventoryUIHolder uiHolder;
    private ItemNoticer itemNoticer;


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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 첫 씬 로드 시 UI를 찾습니다.
        FindAndRegisterUIElements();
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드되면 UI 요소들을 다시 찾습니다.
        FindAndRegisterUIElements();
    }

    private void FindAndRegisterUIElements()
    {
        uiHolder = FindFirstObjectByType<InventoryUIHolder>();
        itemNoticer = FindFirstObjectByType<ItemNoticer>();

        if (uiHolder == null)
            Debug.Log("[InventoryUIManager] 현재 씬에 InventoryUIHolder가 없습니다.");
        else
            Debug.Log("[InventoryUIManager] 현재 씬의 인벤토리 UI를 성공적으로 찾았습니다.");

        if (itemNoticer == null)
            Debug.LogWarning("[InventoryUIManager] 현재 씬에 ItemNoticer가 없습니다.");
    }

    /// <summary>
    /// gridParent 안에 있는 이전에 만들어진 슬롯들을 전부 삭제 및 ownedItems 순회 프리팹 생성
    /// </summary>
    void PopulateInventory()
    {
        if (uiHolder == null || uiHolder.gridParent == null) return;

        foreach (Transform child in uiHolder.gridParent)
            Destroy(child.gameObject);

        foreach (var item in ownedItems)
        {
            GameObject slot = Instantiate(itemSlotPrefab, uiHolder.gridParent);
            slot.GetComponent<Image>().sprite = item.artwork;

            Button btn = slot.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowItemDetail(item));
        }
    }

    void ShowItemDetail(Item item)
    {
        if (uiHolder == null) return;

        uiHolder.detailImage.sprite = item.artwork;
        uiHolder.detailName.text = item.itemName;
        uiHolder.detailEffect.text = item.itemEffect;
        uiHolder.detailDescription.text = item.description;
    }
    // Open/Close
    public void OpenInventoryPanel()
    {
        if (uiHolder != null && uiHolder.inventoryPanel != null)
        {
            uiHolder.inventoryPanel.SetActive(true);
            PopulateInventory(); // 열 때 새로고침할 경우
        }
    }
    public void CloseInventoryPanel()
    {
        if (uiHolder != null && uiHolder.inventoryPanel != null)
            uiHolder.inventoryPanel.SetActive(false);
    }

    public void AddItem(Item newItem)
    {
        if (!ownedItems.Contains(newItem))
        {
            ownedItems.Add(newItem);
            Debug.Log($"아이템 '{newItem.itemName}'을(를) 획득했습니다.");
            
            // 캐싱해둔 ItemNoticer 인스턴스를 사용합니다.
            if (itemNoticer != null)
            {
                itemNoticer.ShowNotification(newItem.itemName);
            }
            else
            {
                Debug.LogError("ItemNoticer 인스턴스를 찾을 수 없어 알림을 표시할 수 없습니다.");
            }
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
        return ownedItems.Any(item => item.ID == itemID);
    }
}
