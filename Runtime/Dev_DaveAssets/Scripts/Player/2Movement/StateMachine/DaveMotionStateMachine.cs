using System.Collections.Generic;
using Dave6.ShooterFramework.Movement.StateMachine.State;
using Unity.VisualScripting;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine
{
    public class DaveMotionStateMachine
    {
        private IMotionState _currentState;
        private Dictionary<string, IMotionState> _states;
        private readonly DaveMovementController _context;

        public DaveMotionStateMachine(DaveMovementController controller)
        {
            _context = controller;
            _states = new Dictionary<string, IMotionState>
            {
                { "Idle", new IdleState(_context, this) }
            };

            ChangeState("Idle");
        }
        public void Update()
        {
            _currentState?.OnUpdate();
        }

        public void ChangeState(string key)
        {
            if (_states.TryGetValue(key, out IMotionState newState))
            {
                _currentState?.OnExit();
                _currentState = newState;
                _currentState.OnEnter();
            }
        }

        public interface IMotionState
        {
            void OnEnter();
            void OnUpdate();
            void OnExit();
        }

    }
}
