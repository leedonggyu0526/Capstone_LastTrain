using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
//using Coffee.UIExtensions; //파티클용

/// <summary>
/// 카드 드래그 전용 스크립트(안정 버전)
/// - 드래그 시작 시 현재 위치 저장
/// - 드랍 실패 시 부드럽게 원위치 복귀
/// - DropZone과의 연동을 위해 OriginalPosition, Reset 메서드 제공
/// </summary>
public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 위치/상호작용 제어용
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;                 // 좌표 변환용(부모 캔버스)

    // 드래그 시작 시점의 위치(원위치)
    private Vector2 originalPosition;
    public Vector2 OriginalPosition => originalPosition;

    // 마우스-카드 거리 보정
    private Vector2 offset;

    // 판정/매칭용 카드 ID (CSV의 cardID 문자열 사용)
    public string cardID;

    // 다른 카드와 동시 드래그 방지 플래그
    public static bool isDragging = false;

    // (호환용) 필수는 아니지만 인스펙터에서 연결 가능
    public GameObject cardPrefab;
    public Transform parentTransform;

    public Animator animator;
    public string triggerName = "Split";
    //public UIParticle splitEffect; //파티클용

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // 자동 연결(없어도 동작엔 영향 없음)
        if (cardPrefab == null) cardPrefab = gameObject;
        if (parentTransform == null) parentTransform = transform.parent;
    }

    /// <summary>드래그 시작: 원위치 저장 + 오프셋 계산 + 레이캐스트 허용</summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;
        isDragging = true;

        originalPosition = rectTransform.anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse
        );
        offset = rectTransform.anchoredPosition - localMouse;

        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;
    }

    /// <summary>드래그 중: 부모 기준 마우스 좌표 + 오프셋만큼 이동</summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse
        );
        rectTransform.anchoredPosition = localMouse + offset;
    }

    /// <summary>
    /// 드래그 종료: DropZone 위가 아니면 원위치 복귀
    /// DropZone 위라면 후속 처리는 DropZone.OnDrop에서 수행
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;

        if (!IsPointerOverDropZone(eventData.pointerEnter))
        {
            ReturnToOriginalPositionSmooth();
        }
    }

    /// <summary>부모 체인을 타고 올라가며 DropZone 태그 존재 여부 확인</summary>
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

    /// <summary>즉시 원위치로 스냅(교체/실패 처리 시 사용)</summary>
    public void ResetToOriginalPositionInstant()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    /// <summary>Lerp 보간으로 자연스러운 복귀 연출</summary>
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
    /// <summary>
    /// 카드 사용 애니메이션 실행 --> 매커니즘 변경되어 사용 안함
    /// </summary>
    // public void PlaySplitAnimation()
    // {
    //     if (animator != null)
    //     {
    //         splitEffect.Play();

    //         animator.SetTrigger("Split");
    //     }
    //     else
    //     {
    //         DestroyMe();
    //     }
    // }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}