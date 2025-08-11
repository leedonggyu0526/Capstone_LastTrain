//카드 보충 담당

using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public CardDeck deck;              // 덱 (ScriptableObject)
    public GameObject cardPrefab;     // 카드 UI 프리팹
    public Transform parentTransform; // 카드가 붙을 부모

    public void SpawnRandomCard(Vector2 position)
    {
        CardData data = deck.GetRandomCard();
        if (data == null) return;

        GameObject card = Instantiate(cardPrefab, parentTransform);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = position;

        // 카드 데이터 적용
        CardDisplay display = card.GetComponent<CardDisplay>();
        display.cardData = data;
        display.RefreshUI();

        // 드래그 정보 세팅
        CardDrag drag = card.GetComponent<CardDrag>();
        drag.cardID = data.GetHashCode(); // 카드 고유 ID (임시)
    }
}
