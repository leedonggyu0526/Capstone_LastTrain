using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
// ScriptableObject 선언 클래스
public class Item : ScriptableObject
{
    public int ID;
    public string itemName;
    public string itemEffect;
    [TextArea(2, 4)] public string description;
    public Sprite artwork;
}
