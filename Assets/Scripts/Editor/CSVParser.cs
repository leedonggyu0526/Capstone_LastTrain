using System.Collections.Generic;
using System.Text;

public static class CSVParser
{
    /// <summary>
    /// CSV 문자열을 행 단위 배열로 파싱합니다. (줄바꿈, 쉼표, 큰따옴표 대응)
    /// </summary>
    /// <param name="csvText">CSV 전체 텍스트</param>
    /// <returns>각 행마다 셀 문자열 배열</returns>
    public static List<string[]> Parse(string csvText)
    {
        var result = new List<string[]>();
        var currentRow = new List<string>();
        var currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < csvText.Length && csvText[i + 1] == '"')
                    {
                        // 따옴표 이스케이프 ("")
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        // 닫는 따옴표
                        inQuotes = false;
                    }
                }
                else
                {
                    currentField.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    currentRow.Add(currentField.ToString());
                    currentField.Clear();
                }
                else if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && i + 1 < csvText.Length && csvText[i + 1] == '\n') i++;

                    currentRow.Add(currentField.ToString());
                    result.Add(currentRow.ToArray());
                    currentRow = new List<string>();
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
        }

        // 마지막 줄 처리
        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField.ToString());
            result.Add(currentRow.ToArray());
        }

        return result;
    }
}
