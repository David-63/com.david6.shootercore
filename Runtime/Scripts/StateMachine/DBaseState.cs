using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;


namespace David6.ShooterCore.StateMachine
{
    public abstract class DBaseState : IDStateProvider
    {
        private bool _isRoot = false;
        private IDContextProvider _context;
        private IDStateMachineProvider _stateMachine;
        private IDStateProvider _superState;
        private IDStateProvider _subState;

        public bool IsRoot { get { return _isRoot; } set { _isRoot = value; } }
        public IDContextProvider Context => _context;
        public IDStateMachineProvider StateMachine => _stateMachine;

        public IDStateProvider SuperState => _superState;
        public IDStateProvider SubState => _subState;

        public DBaseState(IDContextProvider context, IDStateMachineProvider stateMachine)
        {
            _context = context;
            _stateMachine = stateMachine;
        }

        public abstract void EnterState();
        public abstract void UpdateSelf();
        public abstract void ExitState();

        public abstract void CheckTransition();

        public abstract void InitializeSubState();

        public void UpdateAll()
        {
            UpdateSelf();
            if (_subState != null)
            {
                _subState.UpdateAll();
            }
        }


        // 어떻게 바꾸지?
        protected void SwitchState(IDStateProvider newState)
        {
            ExitState();
            Log.WhatHappend("Entering " + newState);
            newState.EnterState();

            if (_isRoot)
            {
                _stateMachine.ChangeState(newState);
            }
            else if (_superState != null)
            {
                SuperState.SetSubState(newState);
            }
        }

        /// <summary>
        /// 부모 세팅
        /// </summary>
        /// <param name="superState"></param>
        public void SetSuperState(IDStateProvider superState)
        {
            _superState = superState;
        }
        /// <summary>
        /// 자식 세팅
        /// </summary>
        /// <param name="subState"></param>
        public void SetSubState(IDStateProvider subState)
        {
            _subState = subState;
            subState.SetSuperState(this);
        }

    }
}