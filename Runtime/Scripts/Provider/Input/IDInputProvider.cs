using System;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDInputProvider
    {
        event Action OnPause;
        event Action OnResume;
        event Action OnPop;
        event Action<Vector2> OnMove;
        event Action<Vector2> OnLook;
        event Action OnStartJump;
        event Action OnStopJump;
        event Action OnStartRun;
        event Action OnStopRun;
        event Action OnStartAim;
        event Action OnStopAim;
        event Action OnStartFire;
        event Action OnStopFire;
        event Action OnStartReload;
        event Action OnStopReload;


        void HandlePause();
        void HandleResume();
    }
}