//CSV 한 줄 → CardData 생성 → CardDisplay에 적용 → UI 카드 프리팹 표시.
//drag.cardID도 문자열(string)로 통일.
using System.Linq;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform parentTransform;

    private string[] allLines; //csv에서 읽어온 카드 데이터

    private void Awake()
    {
        // Resources/CardData.csv (헤더 1줄 + 데이터 N줄)
        var csv = Resources.Load<TextAsset>("CardData");
        if (csv != null)
        {
            allLines = csv.text.Split('\n').Skip(1).ToArray(); // 헤더 빼고 저장
        }
    }

    // 카드 덱에서 무작위 카드 1장을 뽑아서 지정된 위치(pos)에 생성
    public void SpawnRandomCard(Vector2 pos)
    {
        if (allLines == null || allLines.Length == 0)
        {
            Debug.LogError("CSV에서 카드 데이터를 불러오지 못했습니다!");
            return;
        }

        // 랜덤 한 줄 뽑기
        string line = allLines[Random.Range(0, allLines.Length)].Trim();
        if (string.IsNullOrEmpty(line)) return;

        SpawnFromCsvLine(line, pos); //카드생성
    }

    // CSV 한 줄 데이터를 CardData로 변환해서 카드 오브젝트 생성
    public void SpawnFromCsvLine(string line, Vector2 pos)
    {
        var cols = line.Split(','); //,로 나누기
        var data = CardCsvLoader.CreateFromCsvRow(cols);//CardData ScriptableObject 생성 및 채우기

        var go = Instantiate(cardPrefab, parentTransform);//카드 프리팹 생성, parentTransform 지정

        var rt = go.GetComponent<RectTransform>();//RectTransform 가져와서 위치 저장
        rt.anchoredPosition = pos;

        var display = go.GetComponent<CardDisplay>(); //카드ui 표시
        display.cardData = data;
        display.RefreshUI();

        var drag = go.GetComponent<CardDrag>(); //CardDrag에 cardID넘기기
        drag.cardID = data.cardID; // 문자열 ID 직접 저장
    }
}
