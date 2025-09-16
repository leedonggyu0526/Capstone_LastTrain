using UnityEngine;
using UnityEngine.UI;

public class AlphaClickLimiter : MonoBehaviour
{
    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.alphaHitTestMinimumThreshold = 0.5f; // 알파값 50% 이상만 클릭됨
            Debug.Log($"[AlphaClickLimiter] 적용됨: {gameObject.name}");
        }
    }
}
