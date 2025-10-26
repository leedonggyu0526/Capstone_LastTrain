// SettingsData.cs
using UnityEngine;
using System;

[Serializable] // 1. 직렬화 가능하도록 선언
public class GameSettings
{
    public float masterVolume = 1.0f;
    public int screenResolutionIndex = 0;
    public bool isFullScreen = true;

    // 데이터의 계층 구조를 직접설정
    public GraphicsSettings graphics = new GraphicsSettings();
}

[Serializable]
public class GraphicsSettings
{
    public int qualityLevel = 3;
    public bool isVSyncOn = true;
}