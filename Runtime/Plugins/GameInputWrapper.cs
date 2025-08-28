using System.Runtime.InteropServices;
using UnityEngine;

public static class GameInputWrapper
{
    const string DLL_NAME = "GameInputWrapper"; // DLL 이름, 확장자 생략

    [DllImport(DLL_NAME)]
    public static extern void InitGameInput();

    [DllImport(DLL_NAME)]
    public static extern void SetVibration(float low, float high, float leftTrigger, float rightTrigger);
}