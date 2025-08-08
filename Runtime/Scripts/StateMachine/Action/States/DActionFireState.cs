using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionFireState : DBaseState
    {
        const string FIRE_KEY = "Action.Fire";

        public DActionFireState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            Context.IsFiring = true;

            // 무기에 따라 초기에 속도 세팅해줌
            Context.AnimatorProvider.SetFireRate(Context.FireRate);
            TryFire();
        }

        public override void UpdateSelf()
        {
            CheckTransition();

            if (Context.CooldownProvider.IsReady(FIRE_KEY))
            {                
                TryFire();
            }
        }

        public override void ExitState()
        {
            Context.IsFiring = false;
        }
        public override void CheckTransition()
        {
            if (!Context.InputFire)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
        }
        public override void InitializeSubState() { }

        void TryFire()
        {
            Context.AnimatorProvider.SetFire();
            Context.CooldownProvider.StartCooldown(FIRE_KEY, 60.0f / Context.FireRate);
        }
    }
}