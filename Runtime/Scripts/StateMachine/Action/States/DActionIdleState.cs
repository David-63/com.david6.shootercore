using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionIdleState : DBaseState
    {
        // 내부에 서브 스테이트 머신 달기?
        public DActionIdleState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            IsRoot = true;
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
        }
        public override void InitializeSubState() {}
    }
}