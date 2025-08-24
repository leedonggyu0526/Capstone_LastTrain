using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public EventSpawner eventSpawner;   // 현재 발생한 이벤트를 관리하는 컴포넌트
    public CardSpawner spawner;         // 카드 덱에서 새 카드를 생성해주는 컴포넌트

    public void OnDrop(PointerEventData eventData)
    {
        // 드래그된 오브젝트(카드)
        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard == null)
        {
            Debug.LogError("드랍된 카드가 없습니다.");
            return;
        }

        // 카드 드래그 컴포넌트
        CardDrag card = droppedCard.GetComponent<CardDrag>();
        if (card == null) return;

        // 이벤트가 없으면 복귀
        if (eventSpawner.currentEventID == -1)
        {
            Debug.Log("이벤트가 없습니다! 카드 복귀.");
            card.ReturnToOriginalPositionSmooth();
            return;
        }

        // 문자열로 단순 비교 (예전 방식)
        string required = eventSpawner.currentEventID.ToString();
        // 디버그: 실제 비교 값 확인
        Debug.Log($"[DropZone] 비교: cardID={card.cardID} vs required={required}");

        // 카드ID(string) vs 이벤트ID(int) → 문자열로 변환하여 비교
        if (card.cardID == required)
        {
            // 이벤트 종료
            eventSpawner.DestroyCurrentEvent();

            // 같은 자리 새 카드 보충을 위해 위치 저장
            Vector2 spawnPos = card.OriginalPosition;

            // 사용한 카드 제거
            Destroy(card.gameObject);

            // 새 카드 보충 (CardSpawner에 SpawnRandomCard(Vector2) 구현되어 있어야 함)
            spawner.SpawnRandomCard(spawnPos);
        }
        else
        {
            Debug.Log("카드가 맞지 않습니다! 카드 복귀.");
            card.ReturnToOriginalPositionSmooth();
        }
    }
}
