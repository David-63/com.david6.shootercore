using UnityEngine;


namespace Dave6.ShooterFramework.Movement.StateMachine.State
{
    public class IdleState : DaveMotionStateMachine.IMotionState
    {
        private readonly DaveMovementController _movementController;
        private readonly DaveMotionStateMachine _motionStateMachine;

        public IdleState(DaveMovementController movementController, DaveMotionStateMachine motionStateMachine)
        {
            _movementController = movementController;
            _motionStateMachine = motionStateMachine;
        }

        public void OnEnter() { Debug.Log("Enter Idle State"); }

        public void OnUpdate()
        {
            // 변경 로직 추가하기
            if (_movementController.MoveInput.magnitude > 0.1f)
            {
                _motionStateMachine.ChangeState("Walk");                
            }
        }

        public void OnExit() { Debug.Log("Exit Idle State"); }
    }
}