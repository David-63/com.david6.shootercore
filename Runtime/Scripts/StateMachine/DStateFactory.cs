using System;
using System.Collections.Generic;
using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine
{
    /// <summary>
    /// State Factory for creating states in the state machine.
    /// </summary>
    public class DStateFactory : IDStateFactoryProvider
    {
        readonly IDContextProvider _context;
        readonly IDStateMachineProvider _stateMachine;

        Dictionary<Type, DBaseState> _stateCache = new Dictionary<Type, DBaseState>();

        public DStateFactory(IDContextProvider context, IDStateMachineProvider stateMachine)
        {
            _context = context;
            _stateMachine = stateMachine;
            _stateCache[typeof(DGroundedState)] = new DGroundedState(_context, _stateMachine);
            _stateCache[typeof(DAirborneState)] = new DAirborneState(_context, _stateMachine);
            _stateCache[typeof(DIdleState)] = new DIdleState(_context, _stateMachine);
            _stateCache[typeof(DWalkState)] = new DWalkState(_context, _stateMachine);
            _stateCache[typeof(DRunState)] = new DRunState(_context, _stateMachine);

        }

        public IDStateProvider Grounded()
        {
            return _stateCache[typeof(DGroundedState)];
        }

        public IDStateProvider Airborne()
        {
            return _stateCache[typeof(DAirborneState)];
        }

        public IDStateProvider Idle()
        {
            return _stateCache[typeof(DIdleState)];
        }

        public IDStateProvider Walk()
        {
            return _stateCache[typeof(DWalkState)];
        }

        public IDStateProvider Run()
        {
            return _stateCache[typeof(DRunState)];
        }

    }
}