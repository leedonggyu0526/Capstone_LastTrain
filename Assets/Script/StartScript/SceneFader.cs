using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // ȭ�� ���� �̹��� �Ҵ�
    public float fadeDuration = 1.0f;
    [Header("�̵��� �� �̸�(��ҹ��� ����)")]
    public string sceneName;

    public void FadeAndLoadScene()
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }
    /// <summary>
    /// �Ҵ��� �̹����� ���̵�ƿ� ��Ű�� IEnumerator
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator FadeOutAndLoad(string sceneName)
    {
        // ����(fadeout)
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

        // �� ��ȯ (�񵿱�� �ϸ� �� �ε巯��)
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}
