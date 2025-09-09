using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardData[] availableCards; // �����Ϳ��� ī�� ������ ���� �� ���
    public CardDisplay[] cardSlots;   // Card1~Card3 ������Ʈ�� �־���

    void Start()
    {
        AssignRandomCards();
    }

    void AssignRandomCards()
    {
        // �����ϰ� 3���� ���� �ٸ� ī�� ����
        CardData[] selectedCards = GetRandomCards(3);

        // ī�� ���Կ� ���� ī�� ����
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].cardData = selectedCards[i];
            cardSlots[i].RefreshUI(); // ī�� UI ������Ʈ �Լ� ȣ��
        }
    }

    // �ߺ� ���� N���� ī�� �̱�
    CardData[] GetRandomCards(int count)
    {
        CardData[] result = new CardData[count];
        System.Collections.Generic.List<CardData> list = new System.Collections.Generic.List<CardData>(availableCards);

        for (int i = 0; i < count; i++)
        {
            int randIndex = Random.Range(0, list.Count);
            result[i] = list[randIndex];
            list.RemoveAt(randIndex); // �ߺ� ����
        }
        return result;
    }
}
