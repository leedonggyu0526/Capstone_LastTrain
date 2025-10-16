using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("카드 덱 참조")]
    public CardDeck cardDeck; // 플레이어가 보유한 카드 데이터

    [Header("카드 UI 슬롯")]
    public CardDisplay[] cardSlots; // 화면에 표시할 카드 3개 (Card1 ~ Card3)

    void Start()
    {
        ShowRandomCards(); // 게임 시작 시 랜덤 카드 표시
    }

    /// <summary>
    /// cardDeck에서 무작위로 3장 가져와 UI 슬롯에 표시
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
    /// 외부에서 카드 추가할 때 호출 예시
    /// </summary>
    public void AddCard(CardData card)
    {
        cardDeck.AddCard(card, 1);
        ShowRandomCards(); // UI 갱신
    }

    /// <summary>
    /// 외부에서 카드 사용할 때 호출 예시
    /// </summary>
    public void UseCard(CardData card)
    {
        cardDeck.RemoveCard(card, 1);
        ShowRandomCards(); // UI 갱신
    }
}
