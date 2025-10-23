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
        data.background = string.IsNullOrEmpty(bgKey) ? null : Resources.Load<Sprite>(bgKey);
        data.artwork = string.IsNullOrEmpty(artKey) ? null : Resources.Load<Sprite>(artKey);

        // 희귀도 (문자열 그대로 저장)
        data.rarity = cols[5].Trim();

        // 가격 추가: CSV 컬럼 길이와 파싱 성공 여부 확인
        data.fuelCost = ParseIntSafe(cols, 6);
        data.foodCost = ParseIntSafe(cols, 7);
        data.partsCost = ParseIntSafe(cols, 8);

        return data;
    }

    // [헬퍼 함수] 배열 범위 초과와 파싱 오류를 안전하게 처리
    private static int ParseIntSafe(string[] cols, int index)
    {
        if (cols.Length > index && int.TryParse(cols[index].Trim(), out int value))
        {
            return value;
        }
        return 0;
    }
}
