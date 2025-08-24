//CSV → CardData 변환 로더(CardCsvLoader) 작성
//경로는 Resources/... 로 통일.
using UnityEngine;

public static class CardCsvLoader
{
    // 한 줄을 읽어서 CardData 인스턴스로 반환
    public static CardData CreateFromCsvRow(string[] cols)
    {
        var data = ScriptableObject.CreateInstance<CardData>();

        data.cardID = cols[0].Trim();
        data.cardName = cols[1].Trim();
        data.description = cols[2].Trim();

        string bgKey = cols[3].Trim(); // 예: UI_image/goldCard
        string artKey = cols[4].Trim(); // 예: UI_image/food
        data.background = string.IsNullOrEmpty(bgKey) ? null : Resources.Load<Sprite>(bgKey);
        data.artwork = string.IsNullOrEmpty(artKey) ? null : Resources.Load<Sprite>(artKey);

        data.rarity = cols[5].Trim();

        if (data.background == null) Debug.LogWarning($"배경 로드 실패: {bgKey}");
        if (data.artwork == null) Debug.LogWarning($"아트워크 로드 실패: {artKey}");

        return data;
    }
}
