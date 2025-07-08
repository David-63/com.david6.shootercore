
using David6.ShooterCore.Provider;


namespace David6.ShooterCore.StateMachine
{
    public abstract class DBaseState  : IDStateProvider
    {
        protected IDContextProvider _context;
        protected DStateFactory _factory;
        protected DBaseState _superState;
        protected DBaseState _subState;

        public DBaseState(IDContextProvider context, DStateFactory factory)
        {
            this._context = context;
            this._factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateSelf();
        public abstract void ExitState();

        public abstract void CheckTransition();

        public abstract void InitializeSubState();

        void UpdateAll() { }
        protected void SwitchState(IDStateProvider newState)
        {
            ExitState();
            newState.EnterState();
            _context.CurrentState = newState;
        }
        protected void SetSuperState() { }
        protected void SetSubState() { }

    }
}