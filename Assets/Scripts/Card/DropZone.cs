// DropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 드롭 처리:
/// - 카드ID == 현재 이벤트ID면: 이벤트 종료 후 같은 카드 오브젝트에 새 카드 데이터로 교체
/// - 불일치/이벤트 없음/셋업 미완: 카드 원위치 복귀
/// - 교체 연출: 페이드 아웃 → 데이터 교체 → 원위치 스냅 → 페이드 인
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public EventSpawner eventSpawner;   // 현재 이벤트 관리자
    public CardDeck playerDeck;         // 플레이어 보유 덱

    [Header("Effect")]
    public float fadeDuration = 0.15f;  // 페이드 연출 시간

    public void OnDrop(PointerEventData eventData)
    {
        var droppedCard = eventData.pointerDrag;
        if (droppedCard == null) return;

        var card = droppedCard.GetComponent<CardDrag>();
        if (card == null) return;

        // 필수 레퍼런스 확인 실패 → 복귀
        if (eventSpawner == null)
        {
            card.ReturnToOriginalPositionSmooth();
            return;
        }

        // 이벤트 없음 → 복귀
        if (eventSpawner.currentEventID == -1)
        {
            card.ReturnToOriginalPositionSmooth();
            return;
        }

        // 카드ID와 이벤트ID 일치 시: 교체 진행
        if (card.cardID == eventSpawner.currentEventID.ToString())
        {
            eventSpawner.DestroyCurrentEvent();
            StartCoroutine(ReplaceCardInPlace(card));
        }
        else
        {
            // 불일치 → 복귀
            card.ReturnToOriginalPositionSmooth();
        }
    }

    /// <summary>
    /// 같은 카드 오브젝트 재사용: 페이드 아웃 → 덱/DB에서 새 데이터 적용 → 원위치 스냅 → 페이드 인
    /// </summary>
    private IEnumerator ReplaceCardInPlace(CardDrag card)
    {
        // 페이드 아웃 준비
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

        // 2) 새 카드ID 선택 (가능하면 현재와 다른 ID)
        string currentID = card.cardID;
        string newID = null;

        if (playerDeck != null)
        {
            for (int i = 0; i < 10; i++)
            {
                newID = playerDeck.GetRandomCardID();
                if (!string.IsNullOrEmpty(newID) && newID != currentID) break;
            }
        }

        // 3) DB에서 새 데이터 조회(실패 시 폴백)
        CardData newData = null;
        if (CardDatabase.Instance != null)
        {
            if (!string.IsNullOrEmpty(newID))
                newData = CardDatabase.Instance.Get(newID);

            // 덱이 비었거나 조회 실패 시 DB 랜덤 폴백
            if (newData == null)
                newData = CardDatabase.Instance.GetRandom();
        }

        // 4) UI/ID 교체(데이터가 있을 때만)
        var display = card.GetComponent<CardDisplay>();
        if (display != null && newData != null)
        {
            display.cardData = newData;
            display.RefreshUI();
            card.cardID = newData.cardID;   // 이후 드랍 판정에 사용
        }

        // 5) 원위치 스냅 (비표시 상태에서)
        card.ResetToOriginalPositionInstant();

        // 6) 페이드 인
        yield return FadeIn(cg);
    }

    /// <summary>카드를 천천히 다시 보이게 함</summary>
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
