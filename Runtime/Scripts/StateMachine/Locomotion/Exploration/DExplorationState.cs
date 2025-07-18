using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DExplorationState : DBaseState
    {
        // 내부에 서브 스테이트 머신 달기?
        public DExplorationState(IDContextProvider context, IDStateMachineProvider stateMachine)
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
            if (Context.InputAim)
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DFocusState)));
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