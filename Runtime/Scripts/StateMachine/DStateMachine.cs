

using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine
{
    public class DStateMachine : IDStateMachineProvider
    {
        private IDStateProvider _currentState;

        public void SetInitialState(IDStateProvider initialState)
        {
            _currentState = initialState;
            _currentState.EnterState();
        }
        public void ChangeState(IDStateProvider newState)
        {
            _currentState.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }

        public void OnUpdate()
        {
            _currentState?.UpdateSelf();
        }

    }
}