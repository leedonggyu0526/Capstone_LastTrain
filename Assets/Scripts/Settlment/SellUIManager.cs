using UnityEngine;
using System.Collections;

public class SellUIManager : UIManager
{
    [Header("판매 UI 패널")]
    public GameObject sellPanel;

    [Header("캔버스 그룹")]
    public CanvasGroup sellCanvasGroup;

    private void Awake()
    {
        if (sellPanel != null)
        {
            sellPanel.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnCloseButtonClick()
    {
        HideSell();
    }

    public void ShowSell()
    {
        if (sellPanel == null) return;

        sellPanel.SetActive(true);

        if (sellCanvasGroup != null)
        {
            sellCanvasGroup.alpha = 0f;
            StopAllAnimations();
            fadeCoroutine = StartCoroutine(FadeIn(sellCanvasGroup));
        }
    }

    public void HideSell()
    {
        if (sellPanel == null) return;

        if (sellCanvasGroup != null)
        {
            StopAllAnimations();
            StartCoroutine(FadeOutAndDeactivate());
        }

        sellPanel.SetActive(false);
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        yield return FadeOut(sellCanvasGroup);
        if (sellPanel != null)
        {   
            sellPanel.SetActive(false);
        }
    }
}
