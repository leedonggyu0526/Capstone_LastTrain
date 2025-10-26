using UnityEngine;

public class Nuclear : MonoBehaviour
{
    [Header("Refs")]
    public GameObject nuclear;
    public GameEnd gameEnd;
    void Start()
    {
        nuclear.SetActive(false);
    }

    public void Bomb()
    {
        Debug.LogError($"[Nuclear] Nuclear launch Detected");
        nuclear.SetActive(true);
        gameEnd.CheckGameEnd();
    }
}
