using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card/Card")]
public class CardData : ScriptableObject
{
    public string cardID;        // CSV에서 읽은 고유 ID
    public string cardName;      // 카드 이름
    [TextArea(2, 4)]
    public string description;   // 카드 설명
    public Sprite artwork;       // 카드 일러스트
    public Sprite background;    // 카드 뒷배경 이미지
    public string rarity;        // CSV에서 읽은 문자열 그대로 (Common, Rare, Epic 등)
}
