using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;


public class CSVtoSO : EditorWindow
{
    TextAsset csvFile;
    string outputFolder = "Assets/data/Item";
    string spriteFolder = "Assets/data/Item/ItemImages";

    [MenuItem("Tools(Custom)/CSV → Item ScriptableObject 변환기")]
    public static void ShowWindow()
    {
        GetWindow<CSVtoSO>("CSV to Item SO");
    }

    void OnGUI()
    {
        GUILayout.Label("CSV 파일 → ScriptableObject 변환기", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV 파일", csvFile, typeof(TextAsset), false);
        outputFolder = EditorGUILayout.TextField("출력 폴더", outputFolder);
        spriteFolder = EditorGUILayout.TextField("Sprite 폴더", spriteFolder);

        if (GUILayout.Button("변환 시작"))
        {
            if (csvFile == null)
            {
                Debug.LogError("CSV 파일을 지정하세요.");
                return;
            }

            CreateItemsFromCSV(csvFile.text);
        }
    }

    void CreateItemsFromCSV(string csvText)
    {
        var rows = CSVParser.Parse(csvText);

        if (rows.Count <= 1)
        {
            Debug.LogError("CSV에 유효한 데이터가 없습니다.");
            return;
        }

        Directory.CreateDirectory(outputFolder);

        for (int i = 1; i < rows.Count; i++) // 첫 줄은 헤더
        {
            string[] row = rows[i];
            if (row.Length < 4) continue;

            int id = int.Parse(row[0]);
            string name = row[1];
            string effect = row[2];
            string desc = row[3];
            string path = $"{spriteFolder}/{id}.png";

            Item item = ScriptableObject.CreateInstance<Item>();
            item.ID = id;
            item.itemName = name;
            item.itemEffect = effect;
            item.description = desc;
            item.artwork = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (item.artwork == null)
            {
                Debug.LogWarning($"스프라이트를 찾을 수 없습니다: {path}");
            }
            string assetPath = $"{outputFolder}/Item_{id}.asset";
            AssetDatabase.CreateAsset(item, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✅ {rows.Count - 1}개의 아이템 ScriptableObject 생성 완료");
    }
}

