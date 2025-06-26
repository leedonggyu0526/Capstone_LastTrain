using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea(2, 4)] public string description;
    public Sprite artwork;       // 카드 일러스트
    public Sprite background;    // 카드 뒷배경 이미지 (추가됨)
    public CardRarity rarity; //카드 등급
    public CardID cardID; //카드 id

}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
  
}

public enum CardID
{
    Water1, //워터1,2,3 불1,2,3

}



