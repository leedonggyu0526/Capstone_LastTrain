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
    // ������ �ִ� �� ����Ʈ
    public List<Item> ownedItems;

    void Start()
    {
        PopulateInventory();
    }
    /// <summary>
    /// gridParent �ȿ� �ִ� ������ ������� ���Ե��� ���� ���� �� ownedItems ��ȸ ������ ����
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

        // ���� ȿ�� (������ �̸��� ���� ���������� �����ص� ��)
        if (item.itemName.Contains("��"))
            detailEffect.text = "<color=green>�ķ� ���귮 +35%</color>";
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
            PopulateInventory(); // �� �� ���ΰ�ħ�� ���
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
            Debug.Log($"������ '{newItem.itemName}'��(��) ȹ���߽��ϴ�.");
        }
        else
        {
            Debug.Log($"�̹� '{newItem.itemName}' �������� ������ �ֽ��ϴ�.");
        }

        PopulateInventory(); // UI ���ΰ�ħ
    }
    /// <summary>
    /// ������ ���� ����
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public bool HasItemByID(int itemID)
    {
        return ownedItems.Exists(item => item.ID == itemID);
    }
}
