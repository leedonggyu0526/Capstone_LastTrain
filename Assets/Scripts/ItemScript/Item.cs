using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
// ScriptableObject ���� Ŭ����
public class Item : ScriptableObject
{
    public int ID;
    public string itemName;
    public string itemEffect;
    [TextArea(2, 4)] public string description;
    public Sprite artwork;
}
