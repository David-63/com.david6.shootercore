using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionIdleState : DBaseState
    {
        public DActionIdleState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
        }

        public override void UpdateSelf()
        {
            CheckTransition();
        }

        public override void ExitState()
        {
        }
        public override void CheckTransition()
        {
            // 전투 입력 체크
            if (Context.ShouldFire())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionFireState)));
            }
            else if (Context.ShouldReload())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionReloadState)));
            }
        }
        public override void InitializeSubState() {}
    }
}