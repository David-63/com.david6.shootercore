using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DLocomotionStateFactory : DBaseStateFactory
    {
        public DLocomotionStateFactory(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) {}
        protected override void RegisterStates()
        {
            StateCache[typeof(DExplorationState)] = new DExplorationState(Context, StateMachine);
            StateCache[typeof(DFocusState)] = new DFocusState(Context, StateMachine);

            StateCache[typeof(DGroundedState)] = new DGroundedState(Context, StateMachine);
            StateCache[typeof(DAirborneState)] = new DAirborneState(Context, StateMachine);

            StateCache[typeof(DExplorationIdleState)] = new DExplorationIdleState(Context, StateMachine);
            StateCache[typeof(DExplorationWalkState)] = new DExplorationWalkState(Context, StateMachine);
            StateCache[typeof(DExplorationRunState)] = new DExplorationRunState(Context, StateMachine);
            
            StateCache[typeof(DFocusIdleState)] = new DFocusIdleState(Context, StateMachine);
            StateCache[typeof(DFocusWalkState)] = new DFocusWalkState(Context, StateMachine);
            StateCache[typeof(DFocusRunState)] = new DFocusRunState(Context, StateMachine);
            
        }

    }
}