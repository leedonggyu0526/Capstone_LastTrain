using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// (이전 안정 버전) 카드 드래그 전용 스크립트
/// - 드래그 시작 시 현재 anchoredPosition을 originalPosition에 저장
/// - 드랍 실패 시 부드럽게 원위치 복귀
/// - 필요 시 즉시 스냅 복귀(ResetToOriginalPositionInstant)
/// - DropZone과의 호환을 위해 OriginalPosition 프로퍼티 제공
/// </summary>
public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;                 // 부모 캔버스 (좌표 변환용)

    private Vector2 originalPosition;      // 드래그 시작 시점의 위치 저장
    public Vector2 OriginalPosition => originalPosition; // DropZone에서 참조

    private Vector2 offset;                // 마우스와 카드 사이의 거리 보정

    public string cardID;                  // 카드 ID (CSV의 cardID 사용)
    public static bool isDragging = false; // 전역 드래그 플래그

    // (예전 호환용) 필요 시 인스펙터에서 연결 가능하지만 현재 로직에선 필수 아님
    public GameObject cardPrefab;
    public Transform parentTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // 자동 연결(없어도 동작엔 영향 없음)
        if (cardPrefab == null) cardPrefab = gameObject;
        if (parentTransform == null) parentTransform = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;
        isDragging = true;

        // 드래그 시작 시점 위치 저장
        originalPosition = rectTransform.anchoredPosition;

        // 부모 기준 마우스 위치 계산 → offset 산출
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse
        );

        offset = rectTransform.anchoredPosition - localMouse;

        // 드래그 동안 다른 UI에 레이캐스트 허용 (드롭 감지 위해)
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // 부모 기준 마우스 좌표로 이동
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse
        );

        rectTransform.anchoredPosition = localMouse + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;

        //  디버그: 마우스 아래 오브젝트 확인
        string under = eventData.pointerEnter ? eventData.pointerEnter.name : "null";
        Debug.Log($"[CardDrag] OnEndDrag pointerEnter={under}");

        //  부모까지 타고 올라가며 DropZone 태그 있는지 확인 (자식 히트도 허용)
        if (!IsPointerOverDropZone(eventData.pointerEnter))
        {
            ReturnToOriginalPositionSmooth();
        }
        // 드롭존 위면 후속 처리는 DropZone.OnDrop에서 실행됨
    }

    // 부모 체인을 타고 올라가며 DropZone 태그를 가진 오브젝트가 있는지 확인
    private bool IsPointerOverDropZone(GameObject go)
    {
        while (go != null)
        {
            if (go.CompareTag("DropZone")) return true;
            var t = go.transform.parent;
            go = t ? t.gameObject : null;
        }
        return false;
    }

    /// <summary>부드럽게 원위치 복귀</summary>
    public void ReturnToOriginalPositionSmooth()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothMove());
    }

    /// <summary>즉시 원위치 스냅 (DropZone 교체 시 사용)</summary>
    public void ResetToOriginalPositionInstant()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    private IEnumerator SmoothMove()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = originalPosition;
        float elapsed = 0f;
        const float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition =
                Vector2.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }
}
