// DropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DropZone : MonoBehaviour, IDropHandler
{
    public EventSpawner eventSpawner;   // 현재 이벤트 관리
    public CardDeck playerDeck;         // 플레이어 보유 덱
    public float fadeDuration = 0.15f;  // 페이드 연출 시간

    public void OnDrop(PointerEventData eventData)
    {
        var droppedCard = eventData.pointerDrag;
        if (droppedCard == null) return;

        var card = droppedCard.GetComponent<CardDrag>();
        if (card == null) return;

        // (안전) eventSpawner 연결 확인
        if (eventSpawner == null)
        {
            Debug.LogError("[DropZone] eventSpawner가 연결되지 않았습니다.");
            card.ReturnToOriginalPositionSmooth();
            return;
        }

        // 이벤트 없음 → 원위치 복귀
        if (eventSpawner.currentEventID == -1)
        {
            card.ReturnToOriginalPositionSmooth();
            return;
        }

        // 카드ID와 이벤트ID(문자열) 비교
        if (card.cardID == eventSpawner.currentEventID.ToString())
        {
            // 성공: 이벤트 종료 + 같은 오브젝트에 "내용만 교체"
            eventSpawner.DestroyCurrentEvent();
            StartCoroutine(ReplaceCardInPlace(card));
        }
        else
        {
            // 불일치 → 부드럽게 원위치
            card.ReturnToOriginalPositionSmooth();
        }
    }

    // 같은 카드 오브젝트 재사용: 숨김 → 데이터 교체 → 원위치 → 표시
    private IEnumerator ReplaceCardInPlace(CardDrag card)
    {
        // 디버그: 현재 참조상태 출력
        Debug.Log($"[DropZone] playerDeck={(playerDeck ? playerDeck.name : "null")} db={(CardDatabase.Instance ? "OK" : "NULL")}");

        var cg = card.GetComponent<CanvasGroup>();
        if (cg == null) cg = card.gameObject.AddComponent<CanvasGroup>();

        // 1) 페이드 아웃
        float t = 0f, dur = fadeDuration;
        float startA = cg.alpha;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startA, 0f, t / dur);
            yield return null;
        }
        cg.alpha = 0f;

        // 2) 덱에서 새 카드 ID 선택 (현재와 다른 ID 우선)
        string currentID = card.cardID;
        string newID = null;

        // (안전) playerDeck 널가드
        if (playerDeck != null)
        {
            for (int i = 0; i < 10; i++)
            {
                newID = playerDeck.GetRandomCardID();
                if (!string.IsNullOrEmpty(newID) && newID != currentID) break;
            }
        }
        else
        {
            Debug.LogError("[DropZone] playerDeck이 연결되지 않았습니다.");
        }

        Debug.Log($"[DropZone] currentID={currentID}, picked newID={newID}");

        // 3) DB에서 데이터 조회 (없으면 폴백)
        CardData newData = null;
        if (CardDatabase.Instance != null)
        {
            if (!string.IsNullOrEmpty(newID))
                newData = CardDatabase.Instance.Get(newID);

            // 덱이 비었거나 조회 실패 시 DB 랜덤 폴백
            if (newData == null)
            {
                Debug.LogWarning("[DropZone] 덱이 비었거나 DB 조회 실패. DB 랜덤으로 폴백 시도");
                newData = CardDatabase.Instance.GetRandom();
            }
        }
        else
        {
            Debug.LogError("[DropZone] CardDatabase.Instance == null (씬에 CardDatabase 배치 필요)");
        }

        // 4) UI/ID 교체
        var display = card.GetComponent<CardDisplay>();
        if (display != null && newData != null)
        {
            display.cardData = newData;
            display.RefreshUI();
            card.cardID = newData.cardID;      // 드랍 판정용 ID도 교체
            Debug.Log($"[DropZone] Replaced to '{newData.cardName}'");
        }
        else
        {
            Debug.LogWarning("[DropZone] display 또는 newData가 null. 교체 실패 → 원위치로만");
        }

        // 5) 보이지 않는 상태에서 원위치로 스냅
        card.ResetToOriginalPositionInstant();

        // 6) 페이드 인
        yield return FadeIn(cg);
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }
}
