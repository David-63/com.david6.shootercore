using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine.State
{
    public class DaveIdleState : DaveMotionStateMachine.MotionState
    {
        public DaveIdleState(DaveMovementController controller, DaveMotionStateMachine stateMachine) : base(controller, stateMachine) { }

        public override void OnEnter()
        {
            _context.TargetSpeed = 0f;
        }
        public override void OnUpdate()
        {
            if (_context.CanJump())
            {
                _stateMachine.ChangeState("Jump");
            }
            else if (!_context.Grounded)
            {
                _stateMachine.ChangeState("Airborne");
            }
            else if (_context.InputDirection.sqrMagnitude > 0.01f)
            {
                if (_context.Sprint)
                {
                    _stateMachine.ChangeState("Run");
                }
                else
                {
                    _stateMachine.ChangeState("Walk");
                }
            }
            _context.CalculateGroundSpeed();
            _context.LastDirection = _context.InputDirection;

        }
        public override void OnExit()
        {
        }
    }
}
