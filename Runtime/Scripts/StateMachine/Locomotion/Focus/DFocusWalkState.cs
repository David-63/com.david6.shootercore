using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DFocusWalkState : DFocusGround
    {
        // 내부에 서브 스테이트 머신 달기?
        public DFocusWalkState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        public override void EnterState()
        {
            Context.TargetSpeed = Context.MovementProfile.WalkSpeed;
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();
            GroundSpeed(deltaTime);
            MoveDirection();
            ApplyCharacterRotation();
            SetAnimationDirection(deltaTime);
        }

        public override void ExitState()
        {
        }
        public override void CheckTransition()
        {
            if (!Context.HasMovementInput())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DFocusIdleState)));
            }
            else if (Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DFocusRunState)));
            }
        }
        public override void InitializeSubState() {}
    }
}