using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{

    public static ItemEffectManager Instance { get; private set; }
    private void Awake() { Instance = this; }

    private List<ItemEffect> activeEffects = new();

    public void ApplyEffect(ItemEffect effect, ResourceType type, float value)
    {
        effect.Add(type, value);
        activeEffects.Add(effect);
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
