using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DExplorationIdleState : DExplorationGround
    {
        // 내부에 서브 스테이트 머신 달기?
        public DExplorationIdleState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        public override void EnterState()
        {
            float idle = 0.0f;
            Context.TargetSpeed = idle;
            Context.AnimatorProvider.SetSpeed(idle);
        }

        public override void UpdateSelf()
        {
            CheckTransition();
            GroundSpeed();
        }

        public override void ExitState()
        {
        }
        public override void CheckTransition()
        {
            if (!Context.HasMovementInput()) return;
            if (!Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DExplorationWalkState)));
            }
            else
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DExplorationRunState)));
            }
        }
        public override void InitializeSubState() {}
    }
}