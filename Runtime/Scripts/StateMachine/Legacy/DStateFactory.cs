using System;
using System.Collections.Generic;
using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine
{
    /// <summary>
    /// State Factory for creating states in the state machine.
    /// </summary>
    public class DStateFactory : IDStateFactoryProvider2
    {
        readonly IDContextProvider _context;
        readonly IDStateMachineProvider2 _stateMachine;

        Dictionary<Type, DBaseState2> _stateCache = new Dictionary<Type, DBaseState2>();

        public DStateFactory(IDContextProvider context, IDStateMachineProvider2 stateMachine)
        {
            _context = context;
            _stateMachine = stateMachine;
            _stateCache[typeof(DGroundedState2)] = new DGroundedState2(_context, _stateMachine);
            _stateCache[typeof(DAirborneState2)] = new DAirborneState2(_context, _stateMachine);
            _stateCache[typeof(DIdleState)] = new DIdleState(_context, _stateMachine);
            _stateCache[typeof(DWalkState)] = new DWalkState(_context, _stateMachine);
            _stateCache[typeof(DRunState)] = new DRunState(_context, _stateMachine);

        }

        public IDStateProvider2 Grounded()
        {
            return _stateCache[typeof(DGroundedState2)];
        }

        public IDStateProvider2 Airborne()
        {
            return _stateCache[typeof(DAirborneState2)];
        }

        public IDStateProvider2 Idle()
        {
            return _stateCache[typeof(DIdleState)];
        }

        public IDStateProvider2 Walk()
        {
            return _stateCache[typeof(DWalkState)];
        }

        public IDStateProvider2 Run()
        {
            return _stateCache[typeof(DRunState)];
        }

    }
}