using UnityEngine;
// 호버링 아웃라인 스크립트 : 스프라이트 렌더러, 콜라이더 사용 객체 전용
[ExecuteInEditMode]
[RequireComponent(typeof(Collider2D))] // OnMouseEnter/Exit를 위해 Collider2D를 필수로 만듭니다.
public class SpriteOutline : MonoBehaviour {
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    private SpriteRenderer spriteRenderer;

    void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void OnDisable() {
        UpdateOutline(false);
    }

    // 마우스 포인터가 Collider 2D 안으로 들어왔을 때 호출됩니다.
    void OnMouseEnter() {
        UpdateOutline(true); // 외곽선을 켭니다.
    }

    // 마우스 포인터가 Collider 2D 밖으로 나갔을 때 호출됩니다.
    void OnMouseExit() {
        UpdateOutline(false); // 외곽선을 끕니다.
    }

    void UpdateOutline(bool outline) {
        // OnDisable에서 호출될 때 spriteRenderer가 null일 수 있으므로 확인합니다.
        if (spriteRenderer == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}