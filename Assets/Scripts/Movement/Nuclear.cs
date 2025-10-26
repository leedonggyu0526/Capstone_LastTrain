using UnityEngine;

public class Nuclear : MonoBehaviour
{
    [Header("Refs")]
    public ResourceManager resourceManager;
    public GameObject nuclear;
    void Start()
    {
        nuclear.SetActive(false);
    }

    public void Bomb()
    {
        Debug.LogError($"[Nuclear] Nuclear launch Detected");
        resourceManager.SetResource(ResourceType.Durability, 0);
        nuclear.SetActive(false);
    }
}
