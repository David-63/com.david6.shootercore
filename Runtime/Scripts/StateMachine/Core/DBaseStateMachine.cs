using David6.ShooterCore.Provider;
using David6.ShooterCore.TickSystem;

namespace David6.ShooterCore.StateMachine
{
    public abstract class DBaseStateMachine : IDStateMachineProvider
    {
        IDStateProvider _currentState;
        IDStateFactoryProvider _factory;

        public IDStateFactoryProvider Factory
        {
            get { return _factory; }
            protected set { _factory = value; }
        }

        public IDStateProvider CurrentState
        {
            get { return _currentState; }
            protected set { _currentState = value; }
        }

        public DBaseStateMachine(IDContextProvider context) => RegisterFactory(context);
        public void SetInitialState(IDStateProvider initialState)
        {
            _currentState = initialState;
            _currentState.EnterState();
        }
        public void ChangeState(IDStateProvider newState) => _currentState = newState;
        public void OnUpdate() => _currentState?.UpdateAll();

        protected abstract void RegisterFactory(IDContextProvider context);
    }
}