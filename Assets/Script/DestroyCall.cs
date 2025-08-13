using UnityEngine;
/// <summary>
/// 외부에서 특정 인스턴스 디스트로이 함수 호출 위함(Animation발 호출 등)
/// </summary>
public class DestroyCall : MonoBehaviour
{
    public CardDrag cd;

    public void CardDestroyCall()
    {
        cd.DestroyMe();
    }


}
