using Dave6.ShooterFramework.Animation;
using Dave6.ShooterFramework.Camera;
using Dave6.ShooterFramework.Input;
using Dave6.ShooterFramework.Movement;
using UnityEngine;


namespace Dave6.ShooterFramework.Core
{
    public class DavePlayerManager : MonoBehaviour
    {
        public DaveInputHandler InputHandler;
        public DaveCameraHandler CameraHandler;
        public DaveMovementController MovementController;
        public DaveAnimatorHandler AnimatorHandler;


        private void Awake()
        {
            InputBinding();
            AnimDataBinding();            
        }

        private void Start()
        {
            MovementController.SetCameraInfoProvider(CameraHandler);
            AnimatorHandler.SetCharacterController(MovementController);
        }

        private void InputBinding()
        {
            InputHandler.OnMove += MovementController.HandleMoveInput;
            InputHandler.OnLook += MovementController.HandleLookInput;
            InputHandler.OnJump += MovementController.HandleJumpInput;
            InputHandler.OnStartSprint += MovementController.HandleStartSprintInput;
            InputHandler.OnStopSprint += MovementController.HandleStopSprintInput;
        }

        private void AnimDataBinding()
        {
            MovementController.OnAnimSpeed += AnimatorHandler.SetSpeed;
            MovementController.OnAnimGround += AnimatorHandler.SetGroundData;
            MovementController.OnAnimDirection += AnimatorHandler.SetDirection;
        }
    }
}