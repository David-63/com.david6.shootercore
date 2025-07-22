using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionReloadState : DBaseState
    {
        // 애니메이션으로 떄우기
        float _reloadTime = 1.5f;
        Coroutine _reloadCoroutine;
        public DActionReloadState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            Context.IsReloadReady = false;
            StartReload();
            Context.AnimatorProvider.SetReload();
            Log.WhatHappend("Reload!");

            // TODO:
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
            if (Context.IsReloadReady)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DActionIdleState)));
            }
        }
        public override void InitializeSubState() { }
        IEnumerator ReloadRoutine()
        {
            // Wait for the jump timeout duration before allowing another jump
            yield return new WaitForSeconds(_reloadTime);
            // Reset the jump cooldown
            Context.IsReloadReady = true;
        }
        void StartReload()
        {
            _reloadCoroutine = Context.ExecuteCoroutine(ReloadRoutine());
        }
        void CancelReload()
        {
            if (_reloadCoroutine != null)
            {
                Context.CancelCoroutine(_reloadCoroutine);
                _reloadCoroutine = null;
                Context.IsJumpReady = false;
            }
        }
    }
}