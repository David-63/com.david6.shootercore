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

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();

            if (!Context.InputFire)
            {
                Context.IsTriggerReleased = true;
            }
        }

        public override void ExitState()
        {
        }
        public override void CheckTransition()
        {
            // 현재 조건은 InputFire가 눌린 여부
            // Fire <-> Idle 상태가 계속 왔다갔다 할 수 있음
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