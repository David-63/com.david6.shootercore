using System;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDInputProvider
    {
        public event Action OnPause;
        public event Action OnResume;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action OnStartJump;
        public event Action OnStopJump;
        public event Action OnStartSprint;
        public event Action OnStopSprint;
        public event Action OnStartAim;
        public event Action OnStopAim;
        public event Action OnStartFire;
        public event Action OnStopFire;
        public event Action OnStartReload;
        public event Action OnStopReload;
    }
}