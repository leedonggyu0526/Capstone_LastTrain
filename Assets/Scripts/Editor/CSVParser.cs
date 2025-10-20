using System.Collections.Generic;
using System.Text;

public static class CSVParser
{
    /// <summary>
    /// CSV ���ڿ��� �� ���� �迭�� �Ľ��մϴ�. (�ٹٲ�, ��ǥ, ū����ǥ ����)
    /// </summary>
    /// <param name="csvText">CSV ��ü �ؽ�Ʈ</param>
    /// <returns>�� �ึ�� �� ���ڿ� �迭</returns>
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
                        // ����ǥ �̽������� ("")
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        // �ݴ� ����ǥ
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

        // ������ �� ó��
        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField.ToString());
            result.Add(currentRow.ToArray());
        }

        return result;
    }
}
