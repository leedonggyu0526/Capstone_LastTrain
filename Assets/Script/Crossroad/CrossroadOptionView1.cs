// Assets/Scripts/CrossroadOptionView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CrossroadOptionView1 : MonoBehaviour
{
    [Header("Prefab References (Drag & Drop)")]
    public Image bg;                   // 자식: BG
    public Image icon;                 // 자식: Icon
    public TextMeshProUGUI titleText;  // 자식: Title
    public TextMeshProUGUI descText;   // 자식: Desc

    private Button btn;

    void Awake()
    {
        // 루트에 Button이 붙어 있어야 클릭이 잡혀요!
        btn = GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogWarning($"[CrossroadOptionView] {name}에 Button이 없습니다. 루트에 Button 컴포넌트를 추가하세요.");
        }
    }

    /// <summary>
    /// 런타임에 데이터/이미지/클릭 동작을 바인딩
    /// </summary>
    public void Bind(Sprite bgSprite, Sprite iconSprite, string title, string desc, UnityAction onClick)
    {
        if (bg) bg.sprite = bgSprite;
        if (icon) icon.sprite = iconSprite;
        if (titleText) titleText.text = title;
        if (descText) descText.text = desc;

        if (icon) icon.preserveAspect = true; // 비율 유지 (Image Type이 Simple이어야 보임)

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            if (onClick != null) btn.onClick.AddListener(onClick);
        }
    }

    /// <summary>
    /// (선택) 카드 재사용/풀링 대비 초기화
    /// </summary>
    public void Clear()
    {
        if (bg) bg.sprite = null;
        if (icon) icon.sprite = null;
        if (titleText) titleText.text = "";
        if (descText) descText.text = "";
        if (btn != null) btn.onClick.RemoveAllListeners();
    }
}
