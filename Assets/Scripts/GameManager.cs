using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    private static bool cameraTopViewMode;
    private static float mouseSensitivity;
    private static bool debugMode;

    public static bool CameraTopViewMode { get => cameraTopViewMode; set => cameraTopViewMode = value; }
    public static float MouseSensitivity { get => mouseSensitivity; set => mouseSensitivity = value; }
    public static bool DebugMode { get => debugMode; set => debugMode = value; }
}
