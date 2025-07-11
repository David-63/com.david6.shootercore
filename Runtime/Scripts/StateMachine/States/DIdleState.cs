using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine
{
    public class DIdleState : DBaseState
    {
        public DIdleState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) {}

        public override void EnterState()
        {
            Context.TargetSpeed = 0.0f;
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
            if (!Context.HasMovementInput()) return;
            if (!Context.InputSprint)
            {
                SwitchState(StateMachine.Factory.Walk());
            }
            else
            {
                SwitchState(StateMachine.Factory.Run());
            }
        }

        public override void InitializeSubState()
        {

        }
    }
}