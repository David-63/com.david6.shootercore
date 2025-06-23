using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine.State
{
    public class DaveAirborneState : DaveMotionStateMachine.MotionState
    {
        public DaveAirborneState(DaveMovementController controller, DaveMotionStateMachine stateMachine) : base(controller, stateMachine) { }

        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
            if (_context.Grounded)
            {
                if (_context.InputDirection.sqrMagnitude > 0.01f)
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
                else
                {
                    _stateMachine.ChangeState("Idle");
                }
            }


            _context.CalculateAirborneSpeed();
        }
        public override void OnExit()
        {
        }

    }
}
