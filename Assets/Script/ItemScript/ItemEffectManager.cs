using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{

    public static ItemEffectManager Instance { get; private set; }
    private void Awake() { Instance = this; }

    private List<IItemEffect> activeEffects = new();

    public void ApplyEffect(IItemEffect effect)
    {
        effect.Apply();
        activeEffects.Add(effect);
    }
    public void RemoveEffect(IItemEffect effect)
    {
        effect.Remove();
        activeEffects.Remove(effect);
    }

}
