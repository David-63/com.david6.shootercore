using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine.State
{
    public class DaveJumpState : DaveMotionStateMachine.MotionState
    {
        public DaveJumpState(DaveMovementController controller, DaveMotionStateMachine stateMachine) : base(controller, stateMachine) { }

        public override void OnEnter()
        {
            _context.PerformJump();
        }
        public override void OnUpdate()
        {
            if (_context.IsAirborne())
            {
                _stateMachine.ChangeState("Airborne");
            }
        }
        public override void OnExit()
        {
            _context.OverrideSpeed(_context._currentSpeed * 1.4f);
        }
    }
}
