using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DActionState : DBaseState
    {
        public DActionState(IDContextProvider context, IDStateMachineProvider stateMachine) : base(context, stateMachine) { }

        public override void EnterState()
        {
            Log.WhatHappend("Entering focus");
        }

        public override void UpdateSelf()
        {

        }

        public override void ExitState()
        {
            Log.WhatHappend("Exiting focus");
        }
        public override void CheckTransition() {}
        public override void InitializeSubState() {}
    }
}