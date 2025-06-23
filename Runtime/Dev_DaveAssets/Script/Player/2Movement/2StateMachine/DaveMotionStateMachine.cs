using System.Collections.Generic;
using Dave6.ShooterFramework.Movement.StateMachine.State;
using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine
{
    public class DaveMotionStateMachine
    {
        public abstract class MotionState
        {
            protected DaveMovementController _context;
            protected DaveMotionStateMachine _stateMachine;

            protected MotionState(DaveMovementController controller, DaveMotionStateMachine stateMachine)
            {
                _context = controller;
                _stateMachine = stateMachine;
            }

            public virtual void OnEnter() { }
            public virtual void OnUpdate() {}
            public virtual void OnExit() {}
        }

        private readonly DaveMovementController _context;
        private Dictionary<string, MotionState> _stateMap;
        private MotionState _currentState;

        public DaveMotionStateMachine(DaveMovementController controller)
        {
            _context = controller;
            _stateMap = new Dictionary<string, MotionState>
            {
                { "Idle", new DaveIdleState(_context, this) },
                { "Walk", new DaveWalkState(_context, this) },
                { "Run", new DaveRunState(_context, this) },
                { "Jump", new DaveJumpState(_context, this) },
                { "Airborne", new DaveAirborneState(_context, this) }
            };

            ChangeState("Idle");
        }
        public void Update()
        {
            _currentState?.OnUpdate();
        }

        public void ChangeState(string key)
        {
            if (_stateMap.TryGetValue(key, out MotionState newState))
            {
                _currentState?.OnExit();
                _currentState = newState;
                _currentState.OnEnter();
                Log.WhatHappend(key);
            }
        }

    }
}
