using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea(2, 4)] public string description;
    public Sprite artwork;       // ī�� �Ϸ���Ʈ
    public Sprite background;    // ī�� �޹�� �̹��� (�߰���)
    public CardRarity rarity; //ī�� ���

}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}


