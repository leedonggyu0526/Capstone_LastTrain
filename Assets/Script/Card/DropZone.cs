using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 드롭존 판정 처리:
/// - 카드 드랍 시 이벤트 ID와 카드 ID 비교
/// - 일치하면 이벤트 종료 + 카드 소모 + 덱에서 보충
/// - 불일치하면 카드 원위치 복귀
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    public EventSpawner eventSpawner;  // 이벤트 관리
    public CardSpawner spawner;        // 카드 생성
    public CardDeck playerDeck;        // 플레이어 덱 (보유 상태)

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard == null) return;

        CardDrag card = droppedCard.GetComponent<CardDrag>();
        if (card == null) return;

        // 이벤트 없음
        if (eventSpawner.currentEventID == -1)
        {
            card.ReturnToOriginalPositionSmooth();
            return;
        }

        // 카드ID와 이벤트ID 비교
        if (card.cardID == eventSpawner.currentEventID.ToString())
        {
            Debug.Log("정상적인 카드 사용! 이벤트 제거 + 보충");

            eventSpawner.DestroyCurrentEvent();

            Vector2 pos = card.OriginalPosition;
            Destroy(card.gameObject);

            // 덱에서 보충
            string newID = playerDeck.GetRandomCardID();
            if (newID != null)
            {
                CardData data = CardDatabase.Instance.Get(newID);
                spawner.SpawnAtPosition(pos, data);
            }
        }
        else
        {
            // 잘못된 카드 → 복귀
            card.ReturnToOriginalPositionSmooth();
        }
    }
}
