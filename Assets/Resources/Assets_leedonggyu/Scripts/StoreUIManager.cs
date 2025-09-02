using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class StoreUIManager : UIManager
{
    [Header("상점 UI 패널")]
    public GameObject storePanel;

    [Header("판매 UI 패널")]
    public GameObject sellPanel;
    
    [Header("개별 카드 부분")]
    public GameObject[] singleCards;
    
    [Header("카드 팩 부분")]
    public GameObject[] cardPacks;
    
    [Header("카드 덱 버튼")]
    public GameObject cardDeckButton;

    [Header("판매 버튼")]
    public GameObject sellButton;

    [Header("상점 캔버스그룹")]
    public CanvasGroup storeCanvasGroup;

    [Header("판매 UI 매니저")]
    public SellUIManager sellUIManager;

    private void Awake()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();
        InitializeStore();
    }
    
    protected override void OnCloseButtonClick()
    {
        // 상점 닫기
        Debug.Log("[StoreUI] Close 버튼 클릭");
        HideStore();
    }
    
    /// <summary>
    /// 상점 초기화
    /// </summary>
    private void InitializeStore()
    {
        //  싱글 카드 버튼 리스너 연결
        if (singleCards != null)
        {
            for (int i = 0; i < singleCards.Length; i++)
            {
                GameObject go = singleCards[i];
                if (go == null) continue;
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    int index = i; // 캡처 변수
                    btn.onClick.AddListener(() => OnSingleCardClicked(index));
                }
                else
                {
                    Debug.LogWarning($"[StoreUI] singleCards[{i}]에 Button 컴포넌트가 없습니다.");
                }
            }
        }

        //  카드 팩 버튼 리스너 연결
        if (cardPacks != null)
        {
            for (int i = 0; i < cardPacks.Length; i++)
            {
                GameObject go = cardPacks[i];
                if (go == null) continue;
                Button btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    int index = i; // 캡처 변수
                    btn.onClick.AddListener(() => OnCardPackClicked(index));
                }
                else
                {
                    Debug.LogWarning($"[StoreUI] cardPacks[{i}]에 Button 컴포넌트가 없습니다.");
                }
            }
        }

        // 덱 버튼 리스너 연결
        if (cardDeckButton != null)
        {
            Button btn = cardDeckButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnCardDeckButtonClicked);
            }
            else
            {
                Debug.LogWarning("[StoreUI] cardDeckButton에 Button 컴포넌트가 없습니다.");
            }
        }

        // 판매 버튼 리스너 연결
        if (sellButton != null)
        {
            Button btn = sellButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnSellButtonClicked);
            }
            else
            {
                Debug.LogWarning("[StoreUI] sellButton에 Button 컴포넌트가 없습니다.");
            }
        }
    }
    
    /// <summary>
    /// 상점 표시
    /// </summary>
    public void ShowStore()
    {
        if (storePanel == null) return;

        // 먼저 패널 활성화
        storePanel.SetActive(true);

        // CanvasGroup이 있으면 알파를 0으로 세팅 후 페이드인
        if (storeCanvasGroup != null)
        {
            storeCanvasGroup.alpha = 0f;
            StopAllAnimations();
            fadeCoroutine = StartCoroutine(FadeIn(storeCanvasGroup));
        }
    }
    
    /// <summary>
    /// 상점 숨기기
    /// </summary>
    public void HideStore()
    {
        if (storePanel == null) return;

        if (storeCanvasGroup != null)
        {
            // 즉시 닫기 시 깜빡임 방지: 패널 비활성은 FadeOut 종료 콜백에서 수행
            StopAllAnimations();
            StartCoroutine(FadeOutAndDeactivate());
        }
        else
        {
            storePanel.SetActive(false);
        }
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        yield return FadeOut(storeCanvasGroup);
        if (storePanel != null)
        {
            storePanel.SetActive(false);
        }
    }

    private void OnSingleCardClicked(int index)
    {
        Debug.Log($"[StoreUI] SingleCard 클릭: index={index}");
        // TODO: 싱글 카드 구매/상세 로직
    }

    private void OnCardPackClicked(int index)
    {
        Debug.Log($"[StoreUI] CardPack 클릭: index={index}");
        // TODO: 카드 팩 구매/미리보기 로직
    }

    private void OnCardDeckButtonClicked()
    {
        Debug.Log("[StoreUI] CardDeck 버튼 클릭");
        // TODO: 덱 UI 열기 로직
    }

    private void OnSellButtonClicked()
    {
        Debug.Log("[StoreUI] Sell 버튼 클릭");
        sellUIManager.ShowSell();
    }
}
