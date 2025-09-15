using UnityEngine;
using UnityEngine.UI;

public class AlphaClickLimiter : MonoBehaviour
{
    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = 0.5f; // ���İ� 50% �̻� Ŭ����
            Debug.Log($"[AlphaClickLimiter] �����: {gameObject.name}");
        }
    }
}
