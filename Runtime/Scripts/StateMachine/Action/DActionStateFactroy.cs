using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionStateFactory : DBaseStateFactory
    {
        public DActionStateFactory(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        protected override void RegisterStates()
        {
            StateCache[typeof(DActionIdleState)] = new DActionIdleState(Context, StateMachine);
            StateCache[typeof(DActionFireState)] = new DActionFireState(Context, StateMachine);
            StateCache[typeof(DActionReloadState)] = new DActionReloadState(Context, StateMachine);
            Log.WhatHappend("ActionState initialization successfully");
        }

    }
}