using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public class DLocomotionStateMachine : DBaseStateMachine
    {
        public DLocomotionStateMachine(IDContextProvider context)
         : base(context) {}
        protected override void RegisterFactory(IDContextProvider context)
        {
            Factory = new DLocomotionStateFactory(context, this);
            CurrentState = Factory.GetState(typeof(DExplorationState));
            CurrentState.EnterState();
        }

    }
}