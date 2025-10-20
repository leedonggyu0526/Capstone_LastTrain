using System.Collections.Generic;
using UnityEngine;
// 직접적 효과 구현
public class ItemEffectManager : MonoBehaviour
{

    public static ItemEffectManager Instance { get; private set; }
    private void Awake() { Instance = this; }

    private List<ItemEffect> activeEffects = new();


    // TODO :조정 영역 
    public void ApplyEffect(ItemEffect effect, ResourceType type, float value)
    {
        effect.Add(type, value);
        activeEffects.Remove(effect);
    }
    public void RemoveEffect(ItemEffect effect, ResourceType type, float value)
    {
        effect.Remove(type, value);
        activeEffects.Remove(effect);
    }
    public void SetEffect(ItemEffect effect, ResourceType type, float value)
    {
        effect.Set(type, value);
        activeEffects.Add(effect);
    }
}
