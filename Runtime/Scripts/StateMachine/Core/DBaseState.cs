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

        protected void SwitchState(IDStateProvider newState)
        {
            ExitState();
            TransitionTo(newState);
        }
        private void TransitionTo(IDStateProvider newState)
        {
            if (_isRoot)
            {
                Log.WhatHappend($"[State Transition] {this.GetType().Name} -> {newState.GetType().Name}");
                // 루트인 경우, 스테이트 머신에 전이 반영
                _stateMachine.ChangeState(newState);
                newState.EnterState();
            }
            else if (_superState != null)
            {
                // 서브인 경우, 부모가 서브 전이를 처리하게 위임
                _superState.SwitchSubState(newState);
            }
        }

        public void SetSuperState(IDStateProvider superState) => _superState = superState;
        public void SetSubState(IDStateProvider subState)
        {
            _subState = subState;
            subState.SetSuperState(this);
            Log.WhatHappend($"[SubState Set] {this.GetType().Name} -> {subState.GetType().Name}");
        }

        public void SwitchSubState(IDStateProvider newState)
        {
            // exit가 중복호출 될 수 있음. trigger flag를 추가한다면?
            _subState?.ExitState(); // 기존의 하위 상태 종료
            SetSubState(newState);  // 다음 상태를 현재 하위상태로 연결
            newState.EnterState();
        }
    }
}