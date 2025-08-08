using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DGroundedState : DBaseState
    {
        public DGroundedState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }
        public override void EnterState()
        {
            InitializeSubState();
            Context.VerticalSpeed = Context.MovementProfile.GroundGravity;
        }

        public override void UpdateSelf()
        {
            CheckTransition();
        }

        public override void ExitState()
        {
            // Cleanup when exiting grounded state
            Context.IsFalling = false;
        }
        public override void CheckTransition()
        {
            if (!Context.IsGrounded || Context.ShouldJump())
            {
                SwitchState(StateMachine.Factory.GetState(typeof(DAirborneState)));
            }
        }
        public override void InitializeSubState()
        {
            if (!Context.HasMovementInput())
            {
                if (SuperState is DExplorationState)
                {
                    SetSubState(StateMachine.Factory.GetState(typeof(DExplorationIdleState)));
                }
                else if (SuperState is DFocusState)
                {
                    SetSubState(StateMachine.Factory.GetState(typeof(DFocusIdleState)));
                }
            }
            else if (!Context.InputSprint)
            {
                if (SuperState is DExplorationState)
                {
                    SetSubState(StateMachine.Factory.GetState(typeof(DExplorationWalkState)));
                }
                else if (SuperState is DFocusState)
                {
                    SetSubState(StateMachine.Factory.GetState(typeof(DFocusWalkState)));
                }
            }
            else
            {
                if (SuperState is DExplorationState)
                {
                    SetSubState(StateMachine.Factory.GetState(typeof(DExplorationRunState)));
                }
                else if (SuperState is DFocusState)
                {
                    SetSubState(StateMachine.Factory.GetState(typeof(DFocusRunState)));
                }
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