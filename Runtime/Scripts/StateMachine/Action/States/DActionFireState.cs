using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionFireState : DBaseState
    {
        // 내부에 서브 스테이트 머신 달기?
        public DActionFireState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        public override void EnterState()
        {
            IsRoot = true;
        }

        public override void UpdateSelf()
        {

        }

        public override void ExitState()
        {
        }
        public override void CheckTransition() {}
        public override void InitializeSubState() {}
    }
}