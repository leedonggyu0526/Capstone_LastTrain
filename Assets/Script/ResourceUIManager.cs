using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public struct ResourceUI
{
    public ResourceType type;   // Fuel, Food, MachinePart, Passenger
    public Image icon;          // Inspector에 드롭다운으로 할당
    public TMP_Text countText;      // “500/1000” 을 표시할 Text
}

public class ResourceUIManager : MonoBehaviour
{
    [Header("자원별 UI 아이콘과 텍스트")]
    [Tooltip("씬에 있는 ResourceUI 컴포넌트들을 담을 리스트입니다. 씬이 로드될 때 자동으로 채워집니다.")]
    private List<ResourceUI> resourceUIs = new List<ResourceUI>();

    public static ResourceUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateSpecificUI;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= UpdateSpecificUI;
        }
    }

    void Start()
    {
        // 첫 씬 로드 시 UI를 찾습니다.
        FindAndRegisterUIElements();
        UpdateAllUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드되면 UI 요소들을 다시 찾고, 전체 UI를 갱신합니다.
        FindAndRegisterUIElements();
        UpdateAllUI();
    }

    /// <summary>
    /// 현재 씬에 있는 모든 ResourceUIHolder를 찾아 리스트에 등록합니다.
    /// </summary>
    private void FindAndRegisterUIElements()
    {
        resourceUIs.Clear();
        ResourceUIHolder[] holders = FindObjectsOfType<ResourceUIHolder>();
        foreach (var holder in holders)
        {
            resourceUIs.Add(holder.ui);
        }
        Debug.Log($"[ResourceUIManager] {resourceUIs.Count}개의 자원 UI를 찾았습니다. {resourceUIs}");
    }

    /// <summary>
    /// 모든 자원 UI를 현재 값으로 갱신합니다.
    /// </summary>
    public void UpdateAllUI()
    {
        foreach (var rui in resourceUIs)
        {
            if (rui.countText == null) continue;
            int current = ResourceManager.Instance.GetResource(rui.type);
            int max = ResourceManager.Instance.GetMaxCapacity(rui.type);
            rui.countText.text = $"{current}/{max}";
        }
    }

    /// <summary>
    /// 특정 자원 타입의 UI만 갱신합니다.
    /// </summary>
    private void UpdateSpecificUI(ResourceType type)
    {
        foreach (var rui in resourceUIs)
        {
            if (rui.type == type && rui.countText != null)
            {
                int current = ResourceManager.Instance.GetResource(rui.type);
                int max = ResourceManager.Instance.GetMaxCapacity(rui.type);
                rui.countText.text = $"{current}/{max}";
                break; // 해당 타입의 UI를 찾았으므로 반복 중단
            }
        }
    }
}
