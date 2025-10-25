// Assets/Scripts/TrainImageCSVLoader.cs  (파일명/클래스명 일치 권장)
using System;
using UnityEngine;

/// <summary>
/// Resources/trainImage/CSV/<csvName>.csv 를 읽어 TrainImageDB에 적재.
/// CSV 포맷: 1열=typeKey, 2열~N열=levelX(헤더에서 숫자 추출), 값=이미지 이름.
/// </summary>
public class TrainImageCSVLoader : MonoBehaviour
{
    public TextAsset trainImageCSV;

    void Awake()
    {
        Debug.Log("[TrainImageCSVLoader] Awake() 실행됨");
        Debug.Log($"[TrainImageCSVLoader] GameObject 이름: {gameObject.name}");
        Debug.Log($"[TrainImageCSVLoader] trainImageCSV 할당 상태: {trainImageCSV != null}");
        
        if (trainImageCSV == null)
        {
            Debug.LogError("[TrainImageCSVLoader] trainImageCSV가 할당되지 않았습니다.");
            Debug.LogError("[TrainImageCSVLoader] 인스펙터에서 TextAsset을 할당해주세요.");
            return;
        }
        
        Debug.Log($"[TrainImageCSVLoader] CSV 파일명: {trainImageCSV.name}");
        Debug.Log($"[TrainImageCSVLoader] CSV 텍스트 길이: {trainImageCSV.text.Length}");

        // 라인 분해 및 정리
        var rawLines = trainImageCSV.text.Split('\n');
        if (rawLines.Length == 0)
        {
            Debug.LogWarning("[TrainImageCSVLoader] CSV가 비어 있습니다.");
            return;
        }

        Debug.Log($"[TrainImageCSVLoader] CSV 라인 수: {rawLines.Length}");

        // 헤더 처리
        string headerLine = rawLines[0].TrimEnd('\r').Trim();
        var headers = headerLine.Split(',');
        Debug.Log($"[TrainImageCSVLoader] 헤더: {string.Join(", ", headers)}");

        int totalEntries = 0;

        // 데이터
        for (int i = 1; i < rawLines.Length; i++)
        {
            string line = rawLines[i].TrimEnd('\r').Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var cols = line.Split(',');
            if (cols.Length == 0) continue;

            string typeKey = cols[0].Trim();
            if (string.IsNullOrEmpty(typeKey)) continue;

            Debug.Log($"[TrainImageCSVLoader] 처리 중인 타입: '{typeKey}'");

            // 각 열: 헤더에서 레벨 추출 → 값은 이미지 파일명(확장자 제거)
            for (int j = 1; j < cols.Length && j < headers.Length; j++)
            {
                string imageName = cols[j].Trim().Trim('"');
                if (string.IsNullOrEmpty(imageName)) continue;

                // 확장자 제거 (Resources.Load는 확장자 없이)
                int dot = imageName.LastIndexOf('.');
                if (dot > 0) imageName = imageName.Substring(0, dot);

                int level = TrainImageDB.ExtractLevelFromHeader(headers[j]);
                if (level < 0)
                {
                    // 헤더가 숫자를 포함하지 않으면 j-1을 레벨로 사용(폴백)
                    level = j - 1;
                }

                Debug.Log($"[TrainImageCSVLoader] 등록: {typeKey} L{level} -> {imageName}");
                TrainImageDB.Set(typeKey, level, imageName);
                totalEntries++;
            }
        }
        
        Debug.Log($"[TrainImageCSVLoader] 총 {totalEntries}개 항목 로드 완료");
        
        // 로딩 완료 후 모든 데이터 확인
        TrainImageDB.debugAllLevels();
        
        Debug.Log($"[TrainImageCSVLoader] Engine 최대 레벨: {TrainImageDB.GetMaxLevel("Engine")}");
    }
    
}
