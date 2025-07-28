using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardData[] availableCards; // 에디터에서 카드 데이터 여러 개 등록
    public CardDisplay[] cardSlots;   // Card1~Card3 오브젝트를 넣어줌

    void Start()
    {
        AssignRandomCards();
    }

    void AssignRandomCards()
    {
        // 랜덤하게 3개의 서로 다른 카드 선택
        CardData[] selectedCards = GetRandomCards(3);

        // 카드 슬롯에 랜덤 카드 배정
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].cardData = selectedCards[i];
            cardSlots[i].RefreshUI(); // 카드 UI 업데이트 함수 호출
        }
    }

    // 중복 없이 N개의 카드 뽑기
    CardData[] GetRandomCards(int count)
    {
        CardData[] result = new CardData[count];
        System.Collections.Generic.List<CardData> list = new System.Collections.Generic.List<CardData>(availableCards);

        for (int i = 0; i < count; i++)
        {
            int randIndex = Random.Range(0, list.Count);
            result[i] = list[randIndex];
            list.RemoveAt(randIndex); // 중복 방지
        }
        return result;
    }
}
