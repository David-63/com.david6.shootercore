using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DFocusState : DBaseState
    {
        // 내부에 서브 스테이트 머신 달기?
        public DFocusState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            InitializeSubState();
        }

        public override void UpdateSelf()
        {
            CheckTransition();
        }

        public override void ExitState() { }
        public override void CheckTransition()
        {
            if (!Context.InputAim)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DExplorationState)));
            }
        }
        public override void InitializeSubState()
        {
            if (Context.IsGrounded)
            {
                SetSubState(StateMachine.Factory.GetState(typeof(DGroundedState)));
            }
            else
            {
                SetSubState(StateMachine.Factory.GetState(typeof(DAirborneState)));
            }
            if (SubState != null)
            {
                Log.WhatHappend($"[SubState Enter] {SubState.GetType().Name}");
                SubState.EnterState();
            }
        }
    }
}