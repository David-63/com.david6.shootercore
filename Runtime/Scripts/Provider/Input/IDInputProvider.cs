using System;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDInputProvider : IDProvider
    {
        event Action OnPause;
        event Action OnResume;
        event Action OnCancelPress, OnCancelRelease;
        event Action OnSubmitPress, OnSubmitRelease;
        event Action<Vector2> OnNavigate;

        event Action<Vector2> OnMove;
        event Action<Vector2> OnLook;
        event Action OnStartJump, OnStopJump;
        event Action OnStartRun, OnStopRun;
        event Action OnStartAim, OnStopAim;
        event Action OnStartFire, OnStopFire;
        event Action OnStartReload, OnStopReload;

        void HandlePause();
        void HandleResume();
    }
}