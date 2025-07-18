using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine.Action
{
    public class DActionStateMachine : DBaseStateMachine
    {
        public DActionStateMachine(IDContextProvider context)
         : base(context) { }

        protected override void RegisterFactory(IDContextProvider context)
        {
            Factory = new DActionStateFactory(context, this);

            CurrentState = Factory.GetState(typeof(DActionIdleState));
            CurrentState.EnterState();
        }
    }
}