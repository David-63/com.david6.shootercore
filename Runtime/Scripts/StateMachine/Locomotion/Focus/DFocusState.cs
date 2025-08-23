using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DFocusState : DBaseState
    {
        const int _upperBodyLayer = 1;
        public DFocusState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { IsRoot = true; }

        public override void EnterState()
        {
            InitializeSubState();
            Context.AnimatorProvider.ActiveUpperbodyLayer();
            Context.RigHandlerProvider.ActiveRig();
        }

        public override void UpdateSelf(float deltaTime)
        {
            CheckTransition();            
        }

        public override void ExitState()
        {
            Context.AnimatorProvider.InactiveUpperbodyLayer();
            Context.RigHandlerProvider.InactiveRig();
        }
        public override void CheckTransition()
        {
            if (!Context.IsFocus)
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
                if (DebugMode)
                {
                    Log.WhatHappend($"[SubState Enter] {SubState.GetType().Name}");
                }
                SubState.EnterState();
            }
        }
    }
}