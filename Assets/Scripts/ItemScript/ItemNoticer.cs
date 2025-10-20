using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ItemNoticer : MonoBehaviour
{
    public Transform notificationParent; // 알림 패널 부모 오브젝트(캔버스 내에 빈 오브젝트로 생성, Vertical Layout Group 추가)
    public GameObject notificationPanel;
    public Image notificationImage;
    public Text notificationText;
    public float displayDuration = 3f;
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        notificationPanel.SetActive(false);
        canvasGroup.alpha = 0f;
    }
public void ShowNotification(string message, Sprite imageSprite = null)
{
    GameObject tempPanel = Instantiate(notificationPanel, notificationParent); // 부모를 notificationParent로 지정
    tempPanel.SetActive(true);

    CanvasGroup tempCanvasGroup = tempPanel.GetComponent<CanvasGroup>();
    if (tempCanvasGroup == null)
        tempCanvasGroup = tempPanel.AddComponent<CanvasGroup>();
    tempCanvasGroup.alpha = 0f;

    Text tempText = tempPanel.GetComponentInChildren<Text>();
    if (tempText != null)
        tempText.text = message;

    Image tempImage = tempPanel.GetComponentInChildren<Image>();
    if (tempImage != null && imageSprite != null)
        tempImage.sprite = imageSprite;

    StartCoroutine(FadeInAndDestroy(tempPanel, tempCanvasGroup));
}

    private IEnumerator FadeInAndDestroy(GameObject panel, CanvasGroup cg)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1f;
        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;
        Destroy(panel);
    }
}

