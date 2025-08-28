#include <gameinput.h>
#include <windows.h>

static IGameInput* g_gameInput = nullptr;
static IGameInputDevice* g_device = nullptr;

extern "C" __declspec(dllexport) void InitGameInput() {
    GameInputCreate(&g_gameInput);
    if (g_gameInput) {
        IGameInputReading* reading = nullptr;
        g_gameInput->GetCurrentReading(GameInputKindGamepad, nullptr, &reading);
        if (reading) {
            IGameInputDevice* device = nullptr;
            reading->GetDevice(&device); // 반드시 포인터 주소로 전달!
            g_device = device;           // 전역에 저장
            reading->Release();          // 메모리 해제
        }
    }
}

extern "C" __declspec(dllexport) void SetVibration(float lowFrequency, float highFrequency, float leftTrigger, float rightTrigger) {
    if (!g_device) return;

    GameInputRumbleParams rumble = {};
    rumble.lowFrequency = lowFrequency;
    rumble.highFrequency = highFrequency;
    rumble.leftTrigger = leftTrigger;
    rumble.rightTrigger = rightTrigger;

    g_device->SetRumbleState(&rumble);
}