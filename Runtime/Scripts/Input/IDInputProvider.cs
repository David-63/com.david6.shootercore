using System;
using UnityEngine;

namespace David6.shootercore.Input
{
    public interface IDInputProvider
    {
        public event Action OnPause;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action OnStartJump;
        public event Action OnStopJump;
        public event Action OnStartSprint;
        public event Action OnStopSprint;
        public event Action OnResume;
    }
}