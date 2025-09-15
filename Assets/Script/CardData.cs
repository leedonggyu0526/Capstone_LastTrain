using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea(2, 4)] public string description;
    public Sprite artwork;       // ī�� �Ϸ���Ʈ
    public Sprite background;    // ī�� �޹�� �̹��� (�߰���)
    public CardRarity rarity; //ī�� ���
    public CardID cardID; //ī�� id

}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
  
}

public enum CardID
{
    Water1, //����1,2,3
    Water2,
    Water3,
    umbrella1, //���
    umbrella2,
    umbrella3,
    track1, //����
    track2, 
    track3,
    urethane1, 
    urethane2, 
    urethane3, //�췹ź
    sprayer1, 
    sprayer2, 
    sprayer3, //�� �л��
    box1, 
    box2,
    box3, //�̳�����
    fire1, 
    fire2, 
    fire3, //��ȭ��
    drill1, 
    drill2, 
    drill3, //�帱
    bed1, 
    bed2, 
    bed3, // ��Ʈ����
    Durability1, 
    Durability2, 
    Durability3, //������


}



