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
}
