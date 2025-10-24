using System.Globalization;
using UnityEngine;

public static class CardCsvLoader
{
    /// <summary>
    /// CSV 한 줄을 읽어서 CardData ScriptableObject 인스턴스로 변환한다.
    /// cols 순서: cardID, cardName, description, background, artwork, rarity
    /// </summary>
    public static CardData CreateFromCsvRow(string[] cols)
    {
        string cardBgDefaultPath = "Sprites/UI_image/";
        string cardArtDefaultPath = "Sprites/UI_image/";
        //  수정 사항 : UI_image 폴더를 Sprites/UI_image 폴더로 변경 및 기본 경로 추가

        // 런타임용 CardData 인스턴스 생성
        var data = ScriptableObject.CreateInstance<CardData>();

        // 기본 정보
        data.cardID = cols[0].Trim();       // 카드 고유 ID
        data.cardName = cols[1].Trim();     // 카드 이름
        data.description = cols[2].Trim();  // 카드 설명

        // 리소스 경로 (Resources 폴더 기준)
        string bgKey = cols[3].Trim();  // 배경 이미지 경로
        string artKey = cols[4].Trim(); // 아트워크 이미지 경로

        // 스프라이트 로드 (없으면 null)
        data.background = string.IsNullOrEmpty(bgKey) ? null : Resources.Load<Sprite>(cardBgDefaultPath + bgKey);
        data.artwork = string.IsNullOrEmpty(artKey) ? null : Resources.Load<Sprite>(cardArtDefaultPath + artKey);

        // 희귀도 (문자열 그대로 저장)
        data.rarity = cols[5].Trim();

        // --- Fuel_price (인덱스 6) ---
        if (cols.Length > 6 && int.TryParse(cols[6].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out int fuelPrice))
        {
            data.Fuel_price = fuelPrice;
        }

        // --- Food_price (인덱스 7) ---
        if (cols.Length > 7 && int.TryParse(cols[7].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out int foodPrice))
        {
            data.Food_price = foodPrice;
        }

        // --- MachinePart_price (인덱스 8) ---
        if (cols.Length > 8 && int.TryParse(cols[8].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out int machinePartPrice))
        {
            data.MachinePart_price = machinePartPrice;
        }

        return data;
    }
}
