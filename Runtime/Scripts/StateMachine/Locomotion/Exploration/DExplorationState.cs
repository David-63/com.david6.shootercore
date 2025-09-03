using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DExplorationState : DBaseState
    {
        bool _focusOn = false;
        public DExplorationState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            InitializeSubState();
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();
        }

        public override void ExitState() { }
        public override void CheckTransition()
        {
            if (_focusOn)
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
                if (DebugMode)
                {
                    Log.WhatHappend($"[SubState Enter] {SubState.GetType().Name}");
                }
                SubState.EnterState();
            }
        }

        public void OnFocusActive() => _focusOn = true;
        public void OnFocusInactive() => _focusOn = false;
    }
}