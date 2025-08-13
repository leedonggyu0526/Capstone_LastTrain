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
            Debug.LogError("카드가 없습니다.");
            return;
        }

        CardDrag card = droppedCard.GetComponent<CardDrag>();

        if (card != null)
        {
            // 이벤트 스포너가 없으면 카드 원래 위치로 이동
            if (eventSpawner.currentEventID == -1)
            {
                Debug.Log("이벤트 스포너가 없습니다! 카드 원래 위치로 이동.");
                card.ReturnToOriginalPositionSmooth(); // 원래 위치로 이동
                return;
            }

            // 카드 ID와 이벤트 ID가 일치하면 이벤트 제거
            if (card.cardID == eventSpawner.currentEventID)
            {
                Debug.Log("이벤트 스포너가 있습니다! 이벤트 제거");
                eventSpawner.DestroyCurrentEvent();
                Destroy(card.gameObject);
            }
            else
            {
                // 카드 ID와 이벤트 ID가 일치하지 않으면 카드 원래 위치로 이동
                Debug.Log("카드 ID와 이벤트 ID가 일치하지 않습니다! 카드 원래 위치로 이동.");
                card.ReturnToOriginalPositionSmooth();
            }
        }
    }
}
