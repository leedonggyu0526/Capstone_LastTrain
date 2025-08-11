using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public EventSpawner eventSpawner;      // 현재 발생한 이벤트를 관리하는 컴포넌트
    public CardSpawner spawner;            // 카드 덱에서 새 카드를 생성해주는 컴포넌트

    public void OnDrop(PointerEventData eventData)
    {
        // 드래그된 오브젝트(카드)를 가져옴
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard == null)
        {
            Debug.LogError("드랍된 카드가 없습니다.");
            return;
        }

        // 카드 드래그 컴포넌트 가져오기
        CardDrag card = droppedCard.GetComponent<CardDrag>();

        if (card != null)
        {
            // 이벤트가 아직 존재하지 않을 경우
            if (eventSpawner.currentEventID == -1)
            {
                Debug.Log("이벤트가 없습니다! 카드 복귀.");
                card.ReturnToOriginalPositionSmooth();
                return;
            }

            // 카드 ID가 이벤트와 일치하는 경우: 카드 사용 성공
            if (card.cardID == eventSpawner.currentEventID)
            {
                Debug.Log("정상적인 카드 사용! 이벤트 제거 및 새 카드 보충");

                // 이벤트 제거
                eventSpawner.DestroyCurrentEvent();

                // 카드를 제거하고 위치 저장
                Vector2 spawnPos = card.OriginalPosition;
                Destroy(card.gameObject);

                // 덱에서 무작위 카드 생성
                spawner.SpawnRandomCard(spawnPos);
            }
            else
            {
                // 카드가 이벤트와 맞지 않을 경우: 원래 위치로 복귀
                Debug.Log("카드가 맞지 않습니다! 카드 복귀.");
                card.ReturnToOriginalPositionSmooth();
            }
        }
    }
}
