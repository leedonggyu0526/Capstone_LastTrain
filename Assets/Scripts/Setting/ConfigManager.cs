// 싱글톤, JSON 파일 로드, 설정 관리 기능을 담당하는 클래스

using UnityEngine;
using System.IO;
public class ConfigManager : MonoBehaviour
{
    private static ConfigManager instance;
    public static ConfigManager Instance // Singleton Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에 오브젝트가 없다면, 새로 생성하고 로드 로직을 실행
                GameObject go = new GameObject("ConfigManager");
                instance = go.AddComponent<ConfigManager>();
                DontDestroyOnLoad(go);
                instance.LoadSettings();
            }
            return instance;
        }
    }

    private GameSettings currentSettings;

    // 외부에서 설정값을 가져갈 때 사용
    public GameSettings Settings => currentSettings;


    void Start()
    {
        // 1. Singleton을 통해 설정 데이터에 접근
        GameSettings settings = ConfigManager.Instance.Settings;

        // 2. 설정값을 사용
        float volume = settings.masterVolume;
        int quality = settings.graphics.qualityLevel;

        Debug.Log($"현재 볼륨: {volume}, 그래픽 품질: {quality}");
    }


    // 이 함수를 통해 JSON 파일을 읽고 객체로 변환합니다.
    private void LoadSettings()
    {
        string jsonFileName = "GameSetting";
        string jsonPath;

        // Resources 폴더에서 TextAsset으로 로드하는 것이 가장 간단합니다.
        TextAsset jsonFile = Resources.Load<TextAsset>("Datas/" + jsonFileName);

        if (jsonFile != null)
        {
            jsonPath = jsonFile.text;
        }
        else
        {
            Debug.LogError($"[ConfigManager] JSON 파일을 찾을 수 없습니다: Datas/{jsonFileName}");
            // 파일이 없을 경우 기본값으로 초기화
            currentSettings = new GameSettings();
            return;
        }

        // 3. JSON 문자열을 C# 객체로 역직렬화 (Deserialization)
        currentSettings = JsonUtility.FromJson<GameSettings>(jsonPath);

        Debug.Log("[ConfigManager] 설정 로드 완료. 마스터 볼륨: " + currentSettings.masterVolume);
    }

    // (저장 기능 추가)
    public void SaveSettings()
    {
        // 1. C# 객체를 JSON 문자열로 직렬화 (Serialization)
        string jsonString = JsonUtility.ToJson(currentSettings, true); // true는 가독성 좋은 형식 지정

        // 2. 파일 저장 경로 설정 (PC 환경에서는 Application.persistentDataPath 권장)
        string filePath = Path.Combine(Application.persistentDataPath, "GameSettings.json");

        try
        {
            // 3. 파일에 쓰기
            File.WriteAllText(filePath, jsonString);
            Debug.Log("설정 저장 성공: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("설정 저장 실패: " + e.Message);
        }
    }
    

}