using System;
using System.Collections.Generic;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine
{
    public abstract class DBaseStateFactory : IDStateFactoryProvider
    {
        readonly IDContextProvider _context;
        readonly IDStateMachineProvider _stateMachine;
        Dictionary<Type, IDStateProvider> _stateCache = new Dictionary<Type, IDStateProvider>();
        public IDContextProvider Context => _context;
        public IDStateMachineProvider StateMachine => _stateMachine;
        public Dictionary<Type, IDStateProvider> StateCache => _stateCache;

        public DBaseStateFactory(IDContextProvider context, IDStateMachineProvider stateMachine)
        {
            _context = context;
            _stateMachine = stateMachine;
            RegisterStates();
        }

        public IDStateProvider GetState(Type stateType)
        {
            if (_stateCache.TryGetValue(stateType, out IDStateProvider state))
            {
                return state;
            }
            else
            {
                Log.WhatHappend($"State of type {stateType.Name} is not registered in this factory.");
                return null;
            }
        }

        protected abstract void RegisterStates();

        public void ActiveStateDebugMode()
        {
            foreach (var state in _stateCache.Values)
            {
                if (state is DBaseState baseState)
                {
                    baseState.DebugMode = true;
                }
            }
            Log.WhatHappend("State debug mode activated for all states in the factory.");

        }

    }
}