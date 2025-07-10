using David6.ShooterCore.Provider;

namespace David6.ShooterCore.StateMachine
{
    public class DWalkState : DBaseState
    {
        public DWalkState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) {}
        
        public override void EnterState()
        {
            Context.TargetSpeed = Context.MovementProfile.MoveSpeed;
        }
        public override void UpdateSelf()
        {
            CheckTransition();
        }
        public override void ExitState()
        {

        }

        public override void CheckTransition()
        {
            if (!Context.HasMovementInput)
            {
                SwitchState(StateMachine.Factory.Idle());
            }
            else if (Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.Run());
            }

        }

        public override void InitializeSubState()
        {

        }
    }
}