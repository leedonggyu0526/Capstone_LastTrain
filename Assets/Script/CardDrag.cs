using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Canvas canvas;

    // ���콺�� ī�� ������ �Ÿ�
    private Vector2 offset;

    public int cardID; // ī�� ID

    public static bool isDragging = false; // ��ü���� �巡�� ������ ���� ����

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // �θ� �� ���� ����� Canvas Ž��
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;

        // �θ� �������� ���콺 ��ġ �޾� offset ���
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePosition
        );

        offset = rectTransform.anchoredPosition - localMousePosition;

        // �巡�� �� raycast ���� �ʵ���
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // ���콺 ��ġ�� �θ� �������� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMousePosition
        );

        // ���콺 ��ġ + offset���� ī�� �̵�
        rectTransform.anchoredPosition = localMousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        canvasGroup.blocksRaycasts = true;

        // ������� �ƴϸ� ����ġ�� �ε巴�� ����
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("DropZone"))
        {
            ReturnToOriginalPositionSmooth();
        }

        isDragging = false;
    }

    public void ReturnToOriginalPositionSmooth()
    {
        StopAllCoroutines(); // �ߺ� �̵� ����
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
