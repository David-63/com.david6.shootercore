using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine
{
    /// <summary>
    /// State Factory for creating states in the state machine.
    /// </summary>
    public class DStateFactory : IDStateFactoryProvider
    {
        private readonly IDContextProvider _context;

        public DStateFactory(IDContextProvider context) => this._context = context;

        public IDStateProvider Grounded()
        {
            return new DGroundedState(_context, this);
        }

        public IDStateProvider Airborne()
        {
            return new DAirborneState(_context, this);
        }

        // public IDStateProvider Idle()
        // {
        //     return new DIdleState(_context, this);
        // }

        // public IDStateProvider Walk()
        // {
        //     return new DWalkState(_context, this);
        // }

        // public IDStateProvider Run()
        // {
        //     return new DRunState(_context, this);
        // }

        // public IDStateProvider Jump()
        // {
        //     return new DJumpState(_context, this);
        // }

    }
}