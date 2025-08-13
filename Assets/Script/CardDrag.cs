using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Canvas canvas;

    // 드래그 오프셋
    private Vector2 offset;

    public int cardID; // 카드 ID

    public static bool isDragging = false; // 드래그 중인지 확인

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 카드 부모 캔버스 찾기
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;

        // 카드 원래 위치에서 드래그 시작 위치 차이 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePosition
        );

        offset = rectTransform.anchoredPosition - localMousePosition;

        // 드래그 중에는 카드 레이캐스트 비활성화
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // 드래그 시작 위치에서 현재 마우스 위치 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePosition
        );

        // 드래그 시작 위치 + 차이 만큼 카드 이동
        rectTransform.anchoredPosition = localMousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        canvasGroup.blocksRaycasts = true;

        // 드래그 종료 시 드랍 영역에 들어가지 않으면 원래 위치로 이동
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("DropZone"))
        {
            ReturnToOriginalPositionSmooth();
        }

        isDragging = false;
    }

    public void ReturnToOriginalPositionSmooth()
    {
        StopAllCoroutines(); // 원래 위치로 이동
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
}
