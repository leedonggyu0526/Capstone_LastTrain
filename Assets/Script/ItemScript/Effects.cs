using UnityEngine;
using static ItemEffectManager;

// 인터페이스 구현체 클래스 모음 : 인터페이스 종속
// 자원 생산량 증가 효과 : ResourceIncrease
public class ResourceIncrease : IItemEffect
{
    private ResourceProducer rp;

    public ResourceIncrease(ResourceProducer target)
    {
        rp = target;  // 생성자 주입
    }
    public void Apply()
    {
        rp.correction += 0.2f;
    }

    public void Remove()
    {
        rp.correction -= 0.2f;
    }
}
