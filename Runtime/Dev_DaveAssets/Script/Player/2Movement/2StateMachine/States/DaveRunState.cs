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
        }
        public override void OnUpdate()
        {
            if (_context.CanJump())
            {
                _stateMachine.ChangeState("Jump");
            }
            else if (_context.IsAirborne())
            {
                _stateMachine.ChangeState("Airborne");
            }
            else if (_context.InputDirection.sqrMagnitude < 0.1f)
            {
                _stateMachine.ChangeState("Idle");
            }
            else if (!_context.Sprint)
            {
                _stateMachine.ChangeState("Walk");
            }

            _context.CalculateGroundSpeed();
            _context.LastDirection = _context.InputDirection;
            _context.CalculateMoveDirection();
        }
        public override void OnExit()
        {
        }
    }
}
