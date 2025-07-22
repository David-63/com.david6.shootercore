using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DFocusIdleState : DFocusGround
    {
        // 내부에 서브 스테이트 머신 달기?
        public DFocusIdleState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        public override void EnterState()
        {
            float idleSpeed = 0.0f;
            Context.TargetSpeed = idleSpeed;
            Context.AnimatorProvider.SetSpeed(idleSpeed);
        }

        public override void UpdateSelf()
        {
            CheckTransition();
            GroundSpeed();
            ApplyCharacterRotation();
            SetAnimationDirection();
        }

        public override void ExitState()
        {
        }
        public override void CheckTransition()
        {
            if (!Context.HasMovementInput()) return;
            if (!Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DFocusWalkState)));
            }
            else
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DFocusRunState)));
            }
        }
        public override void InitializeSubState() {}
    }
}