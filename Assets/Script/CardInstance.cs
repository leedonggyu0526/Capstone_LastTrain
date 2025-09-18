[System.Serializable]
public class CardInstance
{
    public CardData cardData;
    public int quantity;

    public CardInstance(CardData data, int quantity)
    {
        this.cardData = data;
        this.quantity = quantity;
    }
}
