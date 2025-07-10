using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine
{
    public class DGroundedState : DBaseState
    {
        public DGroundedState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine)
        {
            IsRoot = true;
            InitializeSubState();
        }
        public override void EnterState()
        {
            // 바닥에 있는 동안 중력은 y -0.1정도로 적용됨
            Context.VerticalSpeed = Context.MovementProfile.GroundGravity;
        }

        public override void UpdateSelf()
        {
            CheckTransition();
        }

        public override void ExitState()
        {
            // Cleanup when exiting grounded state
            Context.TryFall();
        }
        public override void CheckTransition()
        {
            if (!Context.IsGrounded || Context.ShouldJump())
            {
                SwitchState(StateMachine.Factory.Airborne());
            }
        }
        public override void InitializeSubState()
        {
            if (!Context.HasMovementInput)
            {
                SetSubState(StateMachine.Factory.Idle());
            }
            else if (!Context.InputSprint)
            {
                SetSubState(StateMachine.Factory.Walk());
            }
            else
            {
                SetSubState(StateMachine.Factory.Run());
            }
        }
    }
}