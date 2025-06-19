using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine.State
{
    public class DaveRunState : DaveMotionStateMachine.MotionState
    {
        public DaveRunState(DaveMovementController controller, DaveMotionStateMachine stateMachine) : base(controller, stateMachine) { }

        public override void OnEnter()
        {
            _context.TargetSpeed = _context.MovementProfile.SprintSpeed;
            Log.WhatHappend("Run Enter");
        }
        public override void OnUpdate()
        {
            if (!_context.Sprint)
            {
                _stateMachine.ChangeState("Walk");
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
            Log.WhatHappend("Run Exit");
        }
    }
}
