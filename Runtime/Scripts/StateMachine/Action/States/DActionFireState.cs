using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionFireState : DBaseState
    {
        bool _shouldLeave = false;
        const string FIRE_KEY = "Action.Fire";

        public DActionFireState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            // 무기 없으면 되돌아가기
            if (Context.CombatHandler.GetCurrentWeapon == null)
            {
                _shouldLeave = true;
                return;
            }
            Context.IsTriggerReleased = false;

            if (Context.CombatHandler.ChamberLoaded)
            {
                Context.CombatHandler.Fire();
                ActionFire();
                Context.FireRoundRumble();
            }
            else
            {
                Context.EmptyChamberRumble();
                _shouldLeave = true;
            }
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();

            if (!Context.CooldownProvider.IsReady(FIRE_KEY)) return;

            if (Context.CombatHandler.ChamberLoaded)
            {
                Context.CombatHandler.Fire();
                ActionFire();
            }
            else
            {
                Context.EmptyChamberRumble();
                _shouldLeave = true;
            }

            if (!Context.InputFire)
            {
                Context.IsTriggerReleased = true;
                _shouldLeave = true;
            }
        }

        public override void ExitState()
        {
            _shouldLeave = false;
        }

        public override void CheckTransition()
        {
            // 트리거 여부와 상관없이 Idle로 전환 가능
            if (_shouldLeave)
            {
                Context.StopRumble(0.1f);
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
        }
        public override void InitializeSubState() { }

        void ActionFire()
        {
            Context.StartFocus();
            Context.AnimatorProvider.SetFire();
            Context.CooldownProvider.StartCooldown(FIRE_KEY, 60.0f / Context.CombatHandler.GetCurrentWeapon.WeaponFrame.FireRate);
        }
    }
}