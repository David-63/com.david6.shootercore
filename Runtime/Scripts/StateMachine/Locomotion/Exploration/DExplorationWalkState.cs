using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DExplorationWalkState : DExplorationGround
    {
        // 내부에 서브 스테이트 머신 달기?
        public DExplorationWalkState(IDContextProvider context, IDStateMachineProvider stateMachine)
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
        }

        public override void ExitState()
        {
        }
        public override void CheckTransition()
        {
            if (!Context.HasMovementInput())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DExplorationIdleState)));
            }
            else if (Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DExplorationRunState)));
            }

        }
        public override void InitializeSubState() {}
    }
}