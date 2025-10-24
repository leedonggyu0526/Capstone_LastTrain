using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardData[] availableCards; // 가능한 카드 데이터 배열
    public CardDisplay[] cardSlots;   // Card1~Card3 카드 슬롯

    void Start()
    {
        AssignRandomCards();
    }

    void AssignRandomCards()
    {
        // 랜덤으로 3장의 카드 선택
        CardData[] selectedCards = GetRandomCards(3);

        // 카드 슬롯에 카드 데이터 할당
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].cardData = selectedCards[i];
            cardSlots[i].RefreshUI(); // 카드 UI 새로고침
        }
    }

    // 랜덤으로 N장의 카드 선택
    CardData[] GetRandomCards(int count)
    {
        CardData[] result = new CardData[count];
        System.Collections.Generic.List<CardData> list = new System.Collections.Generic.List<CardData>(availableCards);

        for (int i = 0; i < count; i++)
        {
            int randIndex = Random.Range(0, list.Count);
            result[i] = list[randIndex];
            list.RemoveAt(randIndex); // 카드 제거
        }
        return result;
    }
}
