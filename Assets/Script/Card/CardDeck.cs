using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardDeck", menuName = "Card/Deck")]
public class CardDeck : ScriptableObject
{
    public List<CardData> cards = new List<CardData>();

    public CardData GetRandomCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("[CardDeck] 카드 덱이 비어 있습니다.");
            return null;
        }

        return cards[Random.Range(0, cards.Count)];
    }

    // 추후 편집 기능 확장용
    public void AddCard(CardData card) => cards.Add(card);
    public void RemoveCard(CardData card) => cards.Remove(card);
}
