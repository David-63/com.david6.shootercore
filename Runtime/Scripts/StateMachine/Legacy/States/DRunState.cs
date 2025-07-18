using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine
{
    public class DRunState : DBaseState2
    {
        public DRunState(IDContextProvider context, IDStateMachineProvider2 stateMachine)
         : base(context, stateMachine) {}
        
        public override void EnterState()
        {
            Context.TargetSpeed = Context.MovementProfile.RunSpeed;
            Context.AnimatorProvider.SetSpeed(Context.MovementProfile.RunSpeed);
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
            if (!Context.HasMovementInput())
            {
                SwitchState(StateMachine.Factory.Idle());
            }
            else if (!Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.Walk());
            }
        }

        public override void InitializeSubState()
        {

        }
    }
}