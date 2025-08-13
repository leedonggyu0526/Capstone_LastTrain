using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Canvas canvas;
    public Animator animator;
    public string triggerName = "Split";


    // 마우스와 카드 사이의 거리
    private Vector2 offset;

    public int cardID; // 카드 ID

    public static bool isDragging = false; // 전체에서 드래그 중인지 여부 공유

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 부모 중 가장 가까운 Canvas 탐색
        canvas = GetComponentInParent<Canvas>();

        if (animator == null)
        {
            // 자식 오브젝트에서 Animator 컴포넌트 검색
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;

        // 부모 기준으로 마우스 위치 받아 offset 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePosition
        );

        offset = rectTransform.anchoredPosition - localMousePosition;

        // 드래그 중 raycast 막지 않도록
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // 마우스 위치를 부모 기준으로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePosition
        );

        // 마우스 위치 + offset으로 카드 이동
        rectTransform.anchoredPosition = localMousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        canvasGroup.blocksRaycasts = true;

        // 드롭존이 아니면 원위치로 부드럽게 복귀
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("DropZone"))
        {
            ReturnToOriginalPositionSmooth();
        }

        isDragging = false;
    }

    public void ReturnToOriginalPositionSmooth()
    {
        StopAllCoroutines(); // 중복 이동 방지
        StartCoroutine(SmoothMove());
    }

    private IEnumerator SmoothMove()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = originalPosition;
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }
    public void PlaySplitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Split");
        }
        else
        {
            DestroyMe();
        }
    }
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
