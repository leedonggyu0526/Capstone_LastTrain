using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // 화면 덮을 이미지 할당
    public float fadeDuration = 1.0f;
    [Header("이동할 씬 이름(대소문자 구별)")]
    public string sceneName;

    public void FadeAndLoadScene()
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }
    /// <summary>
    /// 할당한 이미지를 페이드아웃 시키는 IEnumerator
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator FadeOutAndLoad(string sceneName)
    {
        //DontDestroyOnLoad 초기화
        DestroyAllDontDestroyOnLoad();  // DontDestroyOnLoad 초기화
        // 암전(fadeout)
        float t = 0;
        Color color = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeImage.color = color;
            yield return null;  
        }
        color.a = 1;
        fadeImage.color = color;
        
        // 씬 전환 (비동기로 하면 더 부드러움)
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    /// <summary>
    /// DontDestroyOnLoad 초기화
    /// </summary>
    private void DestroyAllDontDestroyOnLoad()
    {
        // DontDestroyOnLoad 씬에 있는 모든 루트 GameObject 찾기
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // 루트 객체만 체크 (부모가 없고, scene.buildIndex가 -1인 DontDestroyOnLoad 씬에 있는 것)
            if (obj.transform.parent == null && obj.scene.buildIndex == -1)
            {
                Debug.Log($"[SceneFader] DontDestroyOnLoad 객체 파괴: {obj.name}");
                Destroy(obj);
            }
        }
    }
}
