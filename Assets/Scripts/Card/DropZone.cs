// DropZone.cs
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public EventSpawner eventSpawner;
    public CardDeck playerDeck; // CardDeck.Instance를 사용하도록 권장

    [Header("Effect")]
    public float fadeDuration = 0.15f;

    public void OnDrop(PointerEventData eventData)
    {
        var droppedCard = eventData.pointerDrag;
        if (droppedCard == null) return;

        var card = droppedCard.GetComponent<CardDrag>();
        if (card == null) return;

        // 1. 필수 레퍼런스 및 이벤트 상태 확인
        if (eventSpawner == null || eventSpawner.currentEventID == -1)
        {
            card.ReturnToOriginalPositionSmooth(); // 이벤트 없음 → 복귀
            return;
        }

        // 2. 카드 ID와 이벤트 ID 일치 확인
        string requiredEventID = eventSpawner.currentEventID.ToString();

        // 카드ID와 이벤트ID 일치 시: 카드 사용 성공
        if (card.cardID == requiredEventID)
        {
            string usedID = card.cardID;

            // 🚨 핵심 로직: CardDeck의 수량을 줄이고 갱신 이벤트를 발생시킵니다.
            CardDeck.Instance.UseCard(usedID);

            // 3. 이벤트 종료
            eventSpawner.DestroyCurrentEvent();

            // 4. 연출 코루틴 호출 (CardSpawner가 이미 데이터를 갱신했으므로 연출만 진행)
            StartCoroutine(ReplaceCardInPlace(card));
        }
        else
        {
            // 5. 불일치 → 복귀
            card.ReturnToOriginalPositionSmooth();
        }
    }

    private IEnumerator ReplaceCardInPlace(CardDrag card)
    {
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

        // 🚨 2) 데이터 갱신 로직은 CardSpawner가 담당했으므로 제거

        // 3) 원위치 스냅 (비표시 상태에서)
        // CardSpawner가 이미 위치를 스냅했지만, 최종 연출을 위해 한 번 더 호출합니다.
        card.ResetToOriginalPositionInstant();

        // 4) 페이드 인
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