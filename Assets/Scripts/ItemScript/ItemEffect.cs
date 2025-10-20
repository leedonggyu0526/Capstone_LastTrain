using UnityEngine;
using static ItemEffectManager;

public interface ItemEffect
{
    void Add(ResourceType type, float value);
    void Remove(ResourceType type, float value);
    void Set(ResourceType type, float value);
}