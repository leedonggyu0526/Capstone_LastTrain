using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public EventSpawner eventSpawner;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard == null)
        {
            Debug.LogError("드랍된 카드가 없습니다.");
            return;
        }

        CardDrag card = droppedCard.GetComponent<CardDrag>();

        if (card != null)
        {
            // 이벤트가 없을 경우 → 카드 복귀
            if (eventSpawner.currentEventID == -1)
            {
                Debug.Log("이벤트가 없습니다! 카드 복귀.");
                card.ReturnToOriginalPositionSmooth(); // 애니메이션 복귀 추천
                return;
            }

            // 카드 ID가 일치하는 경우 → 성공
            if (card.cardID == eventSpawner.currentEventID)
            {
                Debug.Log("정상적인 카드 사용! 이벤트 제거");
                card.PlaySplitAnimation();
                eventSpawner.DestroyCurrentEvent();
            }
            else
            {
                // 카드 ID가 다를 경우 → 카드 복귀
                Debug.Log("카드가 맞지 않습니다! 카드 복귀.");
                card.ReturnToOriginalPositionSmooth();
            }
        }
    }
}
