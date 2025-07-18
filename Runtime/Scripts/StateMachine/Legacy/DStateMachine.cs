

using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine
{
    public class DStateMachine : IDStateMachineProvider2
    {
        private IDStateFactoryProvider2 _factory;
        private IDStateProvider2 _currentState;

        public IDStateFactoryProvider2 Factory
        {
            get { return _factory; }
            private set { _factory = value; }
        }

        public IDStateProvider2 CurrentState
        {
            get { return _currentState; }
            private set { _currentState = value; }
        }

        public DStateMachine(IDContextProvider context)
        {
            _factory = new DStateFactory(context, this);
        }

        public void InitializeStateMachine()
        {
            SetInitialState(_factory.Grounded());
        }
        

        public void SetInitialState(IDStateProvider2 initialState)
        {
            _currentState = initialState;
            _currentState.EnterState();
        }
        public void ChangeState(IDStateProvider2 newState)
        {
            //_currentState?.ExitState();
            _currentState = newState;
            
            //_currentState.EnterState();
        }

        public void OnUpdate()
        {
            _currentState?.UpdateAll();
        }

    }
}