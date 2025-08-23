using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Action
{
    /*
        종료 조건을 애니메이션 충족으로 하던가 해야함
        나중가서는 캔슬 기능도 추가해야함
    */
    public class DActionReloadState : DBaseState
    {
        const string RELOAD_KEY = "Action.Reload";
        float _reloadTime = 1.5f;   // 애니메이션 클립 시간을 시용할 예정 (사실상 쿨다운도 사용 안하지)

        public DActionReloadState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            Context.StartFocus();
            Context.CooldownProvider.StartCooldown(RELOAD_KEY, _reloadTime);
            Context.AnimatorProvider.SetReload();
            Context.RigHandlerProvider.InactiveRig();
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();
        }

        public override void ExitState()
        {
            Context.StartFocus();
            Context.RigHandlerProvider.ActiveRig();
        }
        public override void CheckTransition()
        {
            if (Context.CooldownProvider.IsReady(RELOAD_KEY))
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
        }
        public override void InitializeSubState() { }
    }
}