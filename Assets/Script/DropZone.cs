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

                // 이벤트 제거
                eventSpawner.DestroyCurrentEvent();

                // 원래 위치와 부모 저장
                Vector2 originalPos = card.OriginalPosition;
                Transform parent = card.parentTransform;
                GameObject cardPrefab = card.cardPrefab;

                // 카드 제거
                Destroy(card.gameObject);

                // 새로운 카드 생성
                GameObject newCard = Instantiate(cardPrefab, parent);
                RectTransform rt = newCard.GetComponent<RectTransform>();
                rt.anchoredPosition = originalPos; // 원래 위치에 배치

                // 카드에 정보 다시 설정 (랜덤 ID 예시)
                CardDrag newCardDrag = newCard.GetComponent<CardDrag>();
                newCardDrag.cardID = Random.Range(0, 100); // 실제 게임에 맞는 ID 부여
                newCardDrag.cardPrefab = cardPrefab;
                newCardDrag.parentTransform = parent;
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
