using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CrossroadOptionView : MonoBehaviour
{
    [Header("Refs")]
    public Image bg;
    public Image icon;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;

    private UnityAction onClick;

    public void Bind(Sprite bgSprite, Sprite iconSprite, string title, string desc, UnityAction clickAction)
    {
        if (bg) bg.sprite = bgSprite;
        if (icon) icon.sprite = iconSprite;
        if (titleText) titleText.text = title;
        if (descText) descText.text = desc;

        // 버튼 이벤트 바인딩
        var btn = GetComponent<Button>();
        if (btn)
        {
            btn.onClick.RemoveAllListeners();
            onClick = clickAction;
            btn.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
