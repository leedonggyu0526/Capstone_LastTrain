using UnityEngine;

/// <summary>
/// 카드 프리팹을 화면에 생성하고 UI를 채워주는 역할.
/// CardData를 받아서 카드 UI를 표시하는 "표시 전용" 클래스.
/// </summary>
public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;      // 카드 프리팹 (CardDisplay, CardDrag 붙어 있어야 함)
    public Transform parentTransform;  // 카드가 배치될 부모 (예: CardPanel)

    /// <summary>
    /// 지정된 위치(pos)에 CardData를 가진 카드를 생성
    /// </summary>
    public GameObject SpawnAtPosition(Vector2 pos, CardData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[CardSpawner] CardData가 null입니다.");
            return null;
        }

        // 프리팹 생성
        GameObject card = Instantiate(cardPrefab, parentTransform);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;

        // UI 적용
        CardDisplay display = card.GetComponent<CardDisplay>();
        display.cardData = data;
        display.RefreshUI();

        // 드래그 설정
        CardDrag drag = card.GetComponent<CardDrag>();
        drag.cardID = data.cardID;

        return card;
    }
}
