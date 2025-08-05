using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CardLoader : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardParent;
    public GameObject cardPanel;

    void Start()
    {
        List<CrossRoadCardData> allCards = LoadCSV("CrossRoadCard_data");

        // 랜덤 개수 (2 또는 3장)
        int count = Random.Range(2, 4);

        // 전체 리스트 섞기
        Shuffle(allCards);

        // 앞에서부터 count개 뽑아서 생성
        for (int i = 0; i < count && i < allCards.Count; i++)
        {
            CreateCardUI(allCards[i]);
        }
    }

    List<CrossRoadCardData> LoadCSV(string fileName)
    {
        List<CrossRoadCardData> result = new List<CrossRoadCardData>();
        TextAsset fileData = Resources.Load<TextAsset>(fileName);
        StringReader reader = new StringReader(fileData.text);

        reader.ReadLine(); // skip header
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] parts = line.Split(',');
            if (parts.Length >= 5)
            {
                CrossRoadCardData data = new CrossRoadCardData()
                {
                    image1 = parts[0],
                    image2 = parts[1],
                    text1 = parts[2],
                    text2 = parts[3],
                    rank = int.Parse(parts[4])
                };
                result.Add(data);
            }
        }
        return result;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    void CreateCardUI(CrossRoadCardData data)
    {
        GameObject obj = Instantiate(cardPrefab, cardParent);
        Transform tf = obj.transform;

        tf.Find("Image1").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Images/" + data.image1);
        tf.Find("Image2").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Images/" + data.image2);
        tf.Find("Text1").GetComponent<TMPro.TextMeshProUGUI>().text = data.text1;
        tf.Find("Text2").GetComponent<TMPro.TextMeshProUGUI>().text = data.text2;

        obj.GetComponent<Button>().onClick.AddListener(() =>
        {
            cardPanel.SetActive(false);
        });

    }
   
}
