using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine.State
{
    public class DaveWalkState : DaveMotionStateMachine.MotionState
    {
        public DaveWalkState(DaveMovementController controller, DaveMotionStateMachine stateMachine) : base(controller, stateMachine) { }

        public override void OnEnter()
        {
            _context.TargetSpeed = _context.MovementProfile.MoveSpeed;
            Log.WhatHappend("Walk Enter");
        }
        public override void OnUpdate()
        {
            if (_context.Sprint)
            {
                _stateMachine.ChangeState("Run");
            }
            if (_context.InputDirection.sqrMagnitude < 0.1f)
            {
                _stateMachine.ChangeState("Idle");
            }

            _context.CalculateMoveSpeed();
            _context.MoveWithRotation();
        }
        public override void OnExit()
        {
            Log.WhatHappend("Walk Exit");
        }

    }
}