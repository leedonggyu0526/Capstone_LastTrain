using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public List<CardInstance> deck = new List<CardInstance>();

    // ī�� �߰�
    public void AddCard(CardData cardData, int amount = 1)
    {
        CardInstance existing = deck.Find(c => c.cardData == cardData);
        if (existing != null)
        {
            existing.quantity += amount;
        }
        else
        {
            deck.Add(new CardInstance(cardData, amount));
        }
    }

    // ī�� ����
    public void RemoveCard(CardData cardData, int amount = 1)
    {
        CardInstance existing = deck.Find(c => c.cardData == cardData);
        if (existing != null)
        {
            existing.quantity -= amount;
            if (existing.quantity <= 0)
                deck.Remove(existing);
        }
    }

    // ���� ���� ī�� �� �����ϰ� 1�� ��ȯ
    public CardData GetRandomCard()
    {
        List<CardData> expandedList = new List<CardData>();
        foreach (var ci in deck)
        {
            for (int i = 0; i < ci.quantity; i++)
                expandedList.Add(ci.cardData);
        }

        if (expandedList.Count == 0)
            return null;

        return expandedList[Random.Range(0, expandedList.Count)];
    }
}
