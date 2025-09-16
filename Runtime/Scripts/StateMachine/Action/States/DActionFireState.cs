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
            // 무기 없으면 되돌아가기
            var (success, currentWeapon) = Context.CombatHandler.TryGetWeapon();
            if (!success) return;

            DoFire();
            // 트리거 상태 갱신
            Context.IsTriggerReleased = false;
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();

            if (!Context.CooldownProvider.IsReady(FIRE_KEY)) return;

            DoFire();

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
            // 트리거 여부와 상관없이 Idle로 전환 가능
            if (!Context.InputFire || !Context.CombatHandler.IsChamberLoaded())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
        }
        public override void InitializeSubState() { }

        void DoFire()
        {
            bool chamberLoad = Context.CombatHandler.IsChamberLoaded();
            int currentRounds = Context.CombatHandler.GetCurrentRounds();
            Log.WhatHappend($"남은 장탄: {currentRounds}. | 약실: {chamberLoad}");

            if (chamberLoad)
            {
                Context.CombatHandler.RequestFocus(Context.CombatHandler.GetFocusDuration);
                Context.CombatHandler.TryShoot();
                Context.CooldownProvider.StartCooldown(FIRE_KEY, 60.0f / Context.CombatHandler.CurrentFireRate);
            }
        }

    }
}