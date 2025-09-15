using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("ī�� �� ����")]
    public CardDeck cardDeck; // �÷��̾ ������ ī�� ������

    [Header("ī�� UI ����")]
    public CardDisplay[] cardSlots; // ȭ�鿡 ǥ���� ī�� 3�� (Card1 ~ Card3)

    void Start()
    {
        ShowRandomCards(); // ���� ���� �� ���� ī�� ǥ��
    }

    /// <summary>
    /// cardDeck���� �������� 3�� ������ UI ���Կ� ǥ��
    /// </summary>
    public void ShowRandomCards()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            CardData randomCard = cardDeck.GetRandomCard();
            cardSlots[i].cardData = randomCard;
            cardSlots[i].RefreshUI();
        }
    }

    /// <summary>
    /// �ܺο��� ī�� �߰��� �� ȣ�� ����
    /// </summary>
    public void AddCard(CardData card)
    {
        cardDeck.AddCard(card, 1);
        ShowRandomCards(); // UI ����
    }

    /// <summary>
    /// �ܺο��� ī�� ����� �� ȣ�� ����
    /// </summary>
    public void UseCard(CardData card)
    {
        cardDeck.RemoveCard(card, 1);
        ShowRandomCards(); // UI ����
    }
}
