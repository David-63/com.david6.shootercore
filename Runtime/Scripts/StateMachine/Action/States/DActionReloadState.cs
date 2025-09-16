using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Action
{
    /*
        종료 조건을 애니메이션 충족으로 하던가 해야함
        나중가서는 캔슬 기능도 추가해야함
    */
    public class DActionReloadState : DBaseState
    {
        //float _reloadTime = 1.5f;

        bool _reloadFinished = false;

        public DActionReloadState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            _reloadFinished = false;
            Context.CombatHandler.RequestFocus(Context.CombatHandler.GetFocusDuration);
            Context.CombatHandler.LockFocus();
            Context.AnimatorProvider.SetReload();
            Context.RigHandlerProvider.InactiveRig();
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();
        }

        public override void ExitState()
        {
            Context.CombatHandler.UnlockFocus();
            Context.RigHandlerProvider.ActiveRig();
        }
        public override void CheckTransition()
        {
            if (_reloadFinished)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
        }
        public override void InitializeSubState() { }

        public void OnChamberLoad(AnimationEvent animationEvent)
        {
            _reloadFinished = true;
        }

    }
}