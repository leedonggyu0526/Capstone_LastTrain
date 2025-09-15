using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDeckUI : MonoBehaviour
{
    public CardDeck playerDeck;           // ī�� �� ����
    public GameObject cardSlotPrefab;     // ī�� ���� ������
    public Transform cardGrid;            // ī����� ���� Grid
    public GameObject deckPanel;          // ��ü �г� (�Ѱ� ���� �뵵)

    // �� ����
    public void OpenDeck()
    {
        deckPanel.SetActive(true);
        RefreshDeckUI();
    }

    // �� �ݱ�
    public void CloseDeck()
    {
        deckPanel.SetActive(false);
    }

    // ī�� �� UI ����
    public void RefreshDeckUI()
    {
        // ���� ī�� ���� �� ����
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        // ���� ���� ���� ī��� ���� ����
        foreach (CardInstance ci in playerDeck.deck)
        {
            GameObject slot = Instantiate(cardSlotPrefab, cardGrid);

            Image cardImage = slot.transform.Find("CardImage").GetComponent<Image>();
            TextMeshProUGUI quantityText = slot.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();

            cardImage.sprite = ci.cardData.artwork;
            quantityText.text = $"x{ci.quantity}";
        }
    }
}
