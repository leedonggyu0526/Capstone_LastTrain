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
    Water1, //워터1,2,3
    Water2,
    Water3,
    umbrella1, //우산
    umbrella2,
    umbrella3,
    track1, //선로
    track2, 
    track3,
    urethane1, 
    urethane2, 
    urethane3, //우레탄
    sprayer1, 
    sprayer2, 
    sprayer3, //물 분사기
    box1, 
    box2,
    box3, //미끼상자
    fire1, 
    fire2, 
    fire3, //소화기
    drill1, 
    drill2, 
    drill3, //드릴
    bed1, 
    bed2, 
    bed3, // 매트리스
    Durability1, 
    Durability2, 
    Durability3, //내구도


}



