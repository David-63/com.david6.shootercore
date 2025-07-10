using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine
{
    /// <summary>
    /// State Factory for creating states in the state machine.
    /// </summary>
    public class DStateFactory : IDStateFactoryProvider
    {
        private readonly IDContextProvider _context;
        private readonly IDStateMachineProvider _stateMachine;

        public DStateFactory(IDContextProvider context, IDStateMachineProvider stateMachine)
        {
            _context = context;
            _stateMachine = stateMachine;
        }

        public IDStateProvider Grounded()
        {
            return new DGroundedState(_context, _stateMachine);
        }

        public IDStateProvider Airborne()
        {
            return new DAirborneState(_context, _stateMachine);
        }

        public IDStateProvider Idle()
        {
            return new DIdleState(_context, _stateMachine);
        }

        public IDStateProvider Walk()
        {
            return new DWalkState(_context, _stateMachine);
        }

        public IDStateProvider Run()
        {
            return new DRunState(_context, _stateMachine);
        }

    }
}