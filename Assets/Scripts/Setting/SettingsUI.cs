// SettingsUI.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하는 경우

public class SettingsUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider masterVolumeSlider;
    // public TMP_Dropdown resolutionDropdown;
    // public Toggle fullScreenToggle;

    private void Start()
    {
        // 씬 로드 시, ConfigManager에서 현재 설정 값을 가져와 UI에 반영
        LoadCurrentSettingsToUI();

        // UI 요소에 이벤트 리스너(Listener) 연결
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        // resolutionDropdown.onValueChanged.AddListener(SetResolution);
        // fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
    }

    // ConfigManager에서 현재 값을 가져와 UI에 설정
    private void LoadCurrentSettingsToUI()
    {
        // ConfigManager가 Singleton으로 구현되어 있다고 가정
        GameSettings settings = ConfigManager.Instance.Settings;

        masterVolumeSlider.value = settings.masterVolume;
        // 해상도는 Dropdown에 맞는 인덱스나 값으로 설정
        // resolutionDropdown.value = settings.screenResolutionIndex;
        // fullScreenToggle.isOn = settings.isFullScreen;
    }

    // SettingsUI.cs (계속)

    // 마스터 볼륨 변경 (Slider)
    public void SetMasterVolume(float value)
    {
        // 1. ConfigManager의 설정 객체 업데이트
        ConfigManager.Instance.Settings.masterVolume = value;

        // 2. 실제 게임 환경에 적용 (예: Unity Audio Mixer의 Volume 파라미터 변경)
        // AudioListener.volume = value; // 간단한 예시. 실제로는 Audio Mixer 사용 권장

        //변경된 설정을 저장
        ConfigManager.Instance.SaveSettings(); 
    }

    // // 해상도 변경 (Dropdown)
    // public void SetResolution(int index)
    // {
    //     // 1. ConfigManager의 설정 객체 업데이트
    //     ConfigManager.Instance.Settings.screenResolutionIndex = index;

    //     // 2. 실제 게임 환경에 적용
    //     // 실제 해상도 정보를 Dropdown 옵션 목록에서 가져와 적용해야 합니다.
    //     Resolution selectedResolution = Screen.resolutions[index];
    //     Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    // }

    // // 전체 화면 토글 변경 (Toggle)
    // public void SetFullScreen(bool isFullScreen)
    // {
    //     // 1. ConfigManager의 설정 객체 업데이트
    //     ConfigManager.Instance.Settings.isFullScreen = isFullScreen;

    //     // 2. 실제 게임 환경에 적용
    //     Screen.fullScreen = isFullScreen;
    // }
}