using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using Unity.Cinemachine;


namespace David6.ShooterCore.StateMachine
{
    public abstract class DBaseState2 : IDStateProvider2
    {
        private bool _isRoot = false;
        private IDContextProvider _context;
        private IDStateMachineProvider2 _stateMachine;
        private IDStateProvider2 _superState;
        private IDStateProvider2 _subState;

        public bool IsRoot { get { return _isRoot; } set { _isRoot = value; } }
        public IDContextProvider Context => _context;
        public IDStateMachineProvider2 StateMachine => _stateMachine;

        public IDStateProvider2 SuperState => _superState;
        public IDStateProvider2 SubState => _subState;

        public DBaseState2(IDContextProvider context, IDStateMachineProvider2 stateMachine)
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

        protected void SwitchState(IDStateProvider2 newState)
        {
            ExitState();
            TransitionTo(newState);
        }

        /// <summary>
        /// 부모 세팅
        /// </summary>
        /// <param name="superState"></param>
        public void SetSuperState(IDStateProvider2 superState)
        {
            _superState = superState;
        }
        /// <summary>
        /// 자식 세팅
        /// </summary>
        /// <param name="subState"></param>
        public void SetSubState(IDStateProvider2 subState)
        {
            _subState = subState;
            subState.SetSuperState(this);
            Log.WhatHappend($"[SubState Set] {this.GetType().Name} -> {subState.GetType().Name}");
        }

        private void TransitionTo(IDStateProvider2 newState)
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

        public void SwitchSubState(IDStateProvider2 newState)
        {
            _subState?.ExitState(); // 기존의 하위 상태 종료
            SetSubState(newState);  // 다음 상태를 현재 하위상태로 연결
            newState.EnterState();
        }

    }
}